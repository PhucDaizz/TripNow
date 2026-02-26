using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class AddRoomInventoryEventHandler : INotificationHandler<AddRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;
        private readonly ILogger<AddRoomInventoryEventHandler> _logger;

        public AddRoomInventoryEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings, ILogger<AddRoomInventoryEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
            _logger = logger;
        }

        public async Task Handle(AddRoomInventoryEvent notification, CancellationToken cancellationToken)
        {
            var roomTypeId = notification.RoomtypeId;

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
            if (config == null)
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
            }

            int currentDefaultStock = config?.DefaultStock ?? 0;
            int newTotalStock = currentDefaultStock + 1;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);

            var existingDates = await _unitOfWork.Inventory.GetExistingDatesAsync(
                roomTypeId,
                today,
                endDate,
                cancellationToken
            );

            var existingDatesSet = new HashSet<DateOnly>(existingDates);

            var newInventories = new List<Domain.Entities.Inventory>();
            var maxGeneratedDate =
                config?.LastGeneratedDate != null
                    ? DateOnly.FromDateTime(config.LastGeneratedDate.Value)
                    : today.AddDays(-1);

            for (int i = 0; i <= _inventorySettings.LookAheadDays; i++)
            {
                var currentDate = today.AddDays(i);

                if (!existingDatesSet.Contains(currentDate))
                {
                    var newInv = Domain.Entities.Inventory.Create(roomTypeId, currentDate, newTotalStock);
                    newInventories.Add(newInv);

                    if (currentDate > maxGeneratedDate)
                    {
                        maxGeneratedDate = currentDate;
                    }
                }
            }

            if (newInventories.Any())
            {
                await _unitOfWork.Inventory.AddRangeAsync(newInventories, cancellationToken);
            }

            if (existingDates.Any())
            {
                await _unitOfWork.Inventory.UpdateTotalStockForDatesAsync(
                    roomTypeId,
                    existingDates, 
                    1,             
                    cancellationToken
                );
            }

            if (config != null)
            {
                config.UpdateDefaultStock(newTotalStock);
                if (config.LastGeneratedDate == null ||
                    maxGeneratedDate > DateOnly.FromDateTime(config.LastGeneratedDate.Value))
                {
                    config.UpdateLastGeneratedDate(maxGeneratedDate);
                }
            }
            else
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
