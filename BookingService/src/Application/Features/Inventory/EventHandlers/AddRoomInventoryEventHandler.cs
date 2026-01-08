using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class AddRoomInventoryEventHandler : INotificationHandler<AddRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;

        public AddRoomInventoryEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
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

            if (newInventories.Any())
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

        }
    }
}
