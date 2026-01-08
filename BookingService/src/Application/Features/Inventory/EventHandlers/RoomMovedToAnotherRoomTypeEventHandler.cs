using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMovedToAnotherRoomTypeEventHandler : INotificationHandler<RoomMovedToAnotherRoomTypeEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomMovedToAnotherRoomTypeEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
