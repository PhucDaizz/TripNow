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
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);
            var roomTypeId = notification.RoomtypeId;

            var existingDates = await _unitOfWork.Inventory.GetExistingDatesAsync(
                roomTypeId,
                today,
                endDate,
                cancellationToken
            );

            var existingDatesSet = new HashSet<DateOnly>(existingDates);

            var newInventories = new List<Domain.Entities.Inventory>();

            for (int i = 0; i <= _inventorySettings.LookAheadDays; i++)
            {
                var currentDate = today.AddDays(i);

                if (!existingDatesSet.Contains(currentDate))
                {
                    var newInv = Domain.Entities.Inventory.Create(roomTypeId, currentDate, 1);
                    newInventories.Add(newInv);
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

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
            if (config != null)
            {
                config.UpdateDefaultStock(config.DefaultStock + 1);
            }
            else
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
