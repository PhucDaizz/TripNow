using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.Booking.EventHandlers.BookingRefund
{
    public class BookingRefundInitiatedDomainEventHandler : INotificationHandler<BookingRefundInitiatedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public BookingRefundInitiatedDomainEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(BookingRefundInitiatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _integrationEventService.PublishAsync<BookingRefundInitiatedDomainEvent>(
                notification,
                "booking.events",
                "topic",
                "booking.refund",
                cancellationToken);
        }
    }
}
