using BookingService.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeDeletedEvent
{
    public class RoomTypeDeletedEventHandler : INotificationHandler<RoomTypeDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoomTypeDeletedEventHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(RoomTypeDeletedEvent notification, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(notification.RoomTypeId, cancellationToken);

            if (exists is not null)
            {
                await _unitOfWork.InventoryConfiguration.DeleteAsync(exists, cancellationToken);
                await _unitOfWork.Inventory.DeleteAllByRoomTypeIdAsync(notification.RoomTypeId, cancellationToken);
                _logger.LogInformation("Deleted InventoryConfiguration and related Inventory records for RoomTypeId: {RoomTypeId}", notification.RoomTypeId);
            }

        }
    }
}
