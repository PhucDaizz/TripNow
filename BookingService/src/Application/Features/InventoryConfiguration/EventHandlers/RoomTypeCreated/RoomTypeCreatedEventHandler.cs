using BookingService.Application.Common.Interfaces;
using MediatR;

namespace BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeCreated
{
    public class RoomTypeCreatedEventHandler : INotificationHandler<RoomTypeCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomTypeCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RoomTypeCreatedEvent notification, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.InventoryConfiguration
                .AnyAsync(notification.RoomTypeId);

            if (exists) return;

            var config = Domain.Entities.InventoryConfiguration.Create(
                notification.HotelId,
                notification.RoomTypeId,
                notification.InitialStock // Thường là 0
            );

            await _unitOfWork.InventoryConfiguration.AddAsync(config, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }



    }
}
