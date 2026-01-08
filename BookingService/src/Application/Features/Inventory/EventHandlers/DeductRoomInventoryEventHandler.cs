using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class DeductRoomInventoryEventHandler : INotificationHandler<DeductRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeductRoomInventoryEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeductRoomInventoryEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            await _unitOfWork.Inventory.UpdateTotalStockBulkAsync(notification.RoomTypeId, today, -1, cancellationToken);
        }
    }
}
