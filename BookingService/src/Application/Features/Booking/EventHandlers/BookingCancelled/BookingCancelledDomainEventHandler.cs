using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.Booking.EventHandlers.BookingCancelled
{
    public class BookingCancelledDomainEventHandler : INotificationHandler<BookingCancelledDomainEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingCancelledDomainEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BookingCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            var roomTypeIds = notification.Items
            .Select(i => i.RoomTypeId)
            .Distinct()
            .ToList();

            var inventories = await _unitOfWork.Inventory
                .GetInventoriesInRangeAsync(
                    roomTypeIds,
                    notification.Fromdate, 
                    notification.ToDate, 
                    cancellationToken);

            foreach (var inventory in inventories)
            {
                var relatedItems = notification.Items
                    .Where(i => i.RoomTypeId == inventory.RoomTypeId);

                foreach (var item in relatedItems)
                {
                    inventory.ReleaseStock(item.Quantity);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
