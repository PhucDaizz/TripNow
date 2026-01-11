using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class DeductRoomInventoryEventHandler : INotificationHandler<DeductRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeductRoomInventoryEventHandler> _logger;

        public DeductRoomInventoryEventHandler(IUnitOfWork unitOfWork, ILogger<DeductRoomInventoryEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeductRoomInventoryEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var roomTypeId = notification.RoomTypeId;

            await _unitOfWork.Inventory.UpdateTotalStockBulkAsync(
                roomTypeId,
                today,
                -1, 
                cancellationToken
            );

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);

            if (config != null)
            {
                int newStock = config.DefaultStock - 1;
                if (newStock < 0) newStock = 0;

                config.UpdateDefaultStock(newStock);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
            }

            
        }
    }
}
