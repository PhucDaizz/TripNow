using BookingService.Application.Common.Interfaces;
using MediatR;

namespace BookingService.Application.Features.InventoryConfiguration.EventHandlers.HotelStatusChangedEvent
{
    public class HotelStatusChangedEventHandler : INotificationHandler<DTOs.InventoryConfiguration.HotelStatusChangedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;

        public HotelStatusChangedEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
        }

        public async Task Handle(DTOs.InventoryConfiguration.HotelStatusChangedEvent notification, CancellationToken cancellationToken)
        {
            var configs = await _unitOfWork.InventoryConfiguration.GetByHotelIdAsync(notification.HotelId);
            if (!configs.Any()) return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (notification.IsClosed)
            {
                // ĐÓNG KHÁCH SẠN

                foreach (var config in configs)
                {
                    config.MarkAsInactive();
                }

                // Khóa Bulk 
                if (notification.FromDate.HasValue && notification.ToDate.HasValue)
                {
                    var dates = GetDateRange(notification.FromDate.Value, notification.ToDate.Value);

                    foreach (var config in configs)
                    {
                        await _unitOfWork.Inventory.UpdateBlockedStockBulkAsync(
                            config.RoomTypeId,
                            dates,
                            config.DefaultStock, // Khóa số lượng bằng tổng phòng
                            cancellationToken
                        );
                    }
                }
            }
            else
            {
                // MỞ CỬA LẠI

                var reopenDate = notification.FromDate ?? today;
                var lookAheadDate = today.AddDays(_inventorySettings.LookAheadDays);

                var newInventories = new List<Domain.Entities.Inventory>();

                foreach (var config in configs)
                {
                    config.MarkAsActive();

                    var datesToUnblock = await _unitOfWork.Inventory.GetDatesInRangeAsync(
                         config.RoomTypeId, reopenDate, lookAheadDate, cancellationToken);

                    if (datesToUnblock.Any())
                    {
                        await _unitOfWork.Inventory.SetBlockedStockToZeroBulkAsync(
                            config.RoomTypeId, datesToUnblock, cancellationToken);
                    }

                    var existingDates = await _unitOfWork.Inventory.GetExistingDatesAsync(
                        config.RoomTypeId, today, lookAheadDate, cancellationToken);

                    var existingSet = existingDates.ToHashSet();

                    for (var d = today; d <= lookAheadDate; d = d.AddDays(1))
                    {
                        if (!existingSet.Contains(d))
                        {
                            newInventories.Add(Domain.Entities.Inventory.Create(config.RoomTypeId, d, config.DefaultStock));
                        }
                    }

                    config.UpdateLastGeneratedDate(lookAheadDate);
                }

                if (newInventories.Any())
                {
                    await _unitOfWork.Inventory.AddRangeAsync(newInventories, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private List<DateOnly> GetDateRange(DateOnly start, DateOnly end)
        {
            var dates = new List<DateOnly>();

            if (start > end) return dates;

            for (var dt = start; dt <= end; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return dates;
        }
    }
}
