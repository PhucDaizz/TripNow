using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMovedToAnotherRoomTypeEventHandler : INotificationHandler<RoomMovedToAnotherRoomTypeEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomMovedToAnotherRoomTypeEventHandler> _logger;

        public RoomMovedToAnotherRoomTypeEventHandler(IUnitOfWork unitOfWork, ILogger<RoomMovedToAnotherRoomTypeEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(RoomMovedToAnotherRoomTypeEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Inventory.UpdateTotalStockBulkAsync(
                    notification.OldRoomType,
                    today,
                    -1,
                    cancellationToken
                );

                await _unitOfWork.Inventory.UpdateTotalStockBulkAsync(
                    notification.NewRoomType,
                    today,
                    1,
                    cancellationToken
                );

                var oldConfig = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(notification.OldRoomType, cancellationToken);
                if (oldConfig != null)
                {
                    int newOldStock = oldConfig.DefaultStock - 1;
                    oldConfig.UpdateDefaultStock(newOldStock < 0 ? 0 : newOldStock);
                }

                var newConfig = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(notification.NewRoomType, cancellationToken);
                if (newConfig != null)
                {
                    newConfig.UpdateDefaultStock(newConfig.DefaultStock + 1);
                }
                else
                {
                    _logger.LogWarning("Missing InventoryConfiguration for Target RoomType: {RoomTypeId}", notification.NewRoomType);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving room inventory from {Old} to {New}", notification.OldRoomType, notification.NewRoomType);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
