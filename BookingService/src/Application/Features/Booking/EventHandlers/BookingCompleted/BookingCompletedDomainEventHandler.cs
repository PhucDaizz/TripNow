using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.Booking.EventHandlers.BookingCompleted
{
    public class BookingCompletedDomainEventHandler : INotificationHandler<BookingCompletedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public BookingCompletedDomainEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(BookingCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new BookingCompletedIntegrationEvent
            {
                BookingId = notification.bookingId,
                HotelId = notification.hotelId,
                Amount = notification.amount
            };

            await _integrationEventService.PublishAsync<BookingCompletedIntegrationEvent>(
                integrationEvent, 
                "booking.events", 
                "topic", 
                "booking.completed", 
                cancellationToken);
        }
    }
}
