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
            if (notification.IsClosed)
            {
                // 1. TẠM DỪNG CẤU HÌNH (Để Job đêm không chạy cho Hotel này nữa)
                var configs = await _unitOfWork.InventoryConfiguration.GetByHotelIdAsync(notification.HotelId);
                foreach (var config in configs)
                {
                    config.MarkAsInactive(); // IsActive = false
                }

                // 2. KHÓA KHO (BLOCK INVENTORY)
                // Nếu có ngày cụ thể (VD: Đóng 2 tháng)
                if (notification.FromDate.HasValue && notification.ToDate.HasValue)
                {
                    var dates = GetDateRange(notification.FromDate.Value, notification.ToDate.Value);

                    foreach (var config in configs)
                    {
                        // Set BlockedStock = 9999 (Hoặc con số cực lớn để đảm bảo Available < 0)
                        await _unitOfWork.Inventory.UpdateBlockedStockBulkAsync(
                            config.RoomTypeId,
                            dates,
                            9999, // Khóa chặt
                            cancellationToken
                        );
                    }
                }
            }
            else //  Mở cửa
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                // Ngày bắt đầu mở bán lại (Do người dùng chọn, VD: 20/02)
                var reopenDate = notification.FromDate ?? today;

                // Cấu hình lookahead (... ngày)
                var lookAheadDate = today.AddDays(_inventorySettings.LookAheadDays);

                var configs = await _unitOfWork.InventoryConfiguration.GetByHotelIdAsync(notification.HotelId);

                foreach (var config in configs)
                {
                    // 1. KÍCH HOẠT LẠI CONFIG
                    config.MarkAsActive();

                    // 2. MỞ KHÓA INVENTORY (UNBLOCK)
                    // Nếu trước đó đóng đến 30/03, giờ mở từ 20/02 -> Cần mở khóa đoạn 20/02 -> 30/03
                    // Ta set BlockedStock = 0 cho TẤT CẢ các ngày từ reopenDate trở về sau

                    // Lấy tất cả ngày có inventory từ reopenDate đến tương lai xa
                    var datesToUnblock = await _unitOfWork.Inventory.GetDatesInRangeAsync(
                         config.RoomTypeId, reopenDate, lookAheadDate, cancellationToken);

                    if (datesToUnblock.Any())
                    {
                        // Set BlockedStock về 0 (Mở bán hoàn toàn)
                        await _unitOfWork.Inventory.SetBlockedStockToZeroBulkAsync(
                            config.RoomTypeId, datesToUnblock, cancellationToken);
                    }

                    // 3. BACKFILL (TẠO BÙ INVENTORY BỊ THIẾU)
                    // Vì trong thời gian đóng cửa, Job không chạy, nên có thể thiếu ngày.
                    var existingDates = await _unitOfWork.Inventory.GetExistingDatesAsync(
                        config.RoomTypeId, today, lookAheadDate, cancellationToken);
                    var existingSet = new HashSet<DateOnly>(existingDates);

                    var newInventories = new List<Domain.Entities.Inventory>();

                    for (var d = today; d <= lookAheadDate; d = d.AddDays(1))
                    {
                        if (!existingSet.Contains(d))
                        {
                            // Tạo ngày mới bị thiếu
                            newInventories.Add(Domain.Entities.Inventory.Create(config.RoomTypeId, d, config.DefaultStock));
                        }
                    }

                    if (newInventories.Any())
                    {
                        await _unitOfWork.Inventory.AddRangeAsync(newInventories, cancellationToken);

                        // Cập nhật lại mốc generated date
                        config.UpdateLastGeneratedDate(lookAheadDate);
                    }
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
