using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.Booking.EventHandlers.BookingCancelledWithOutPay
{
    public class BookingCancelledWithOutPayDomainEventHandler : INotificationHandler<BookingCancelledWithOutPayDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public BookingCancelledWithOutPayDomainEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(BookingCancelledWithOutPayDomainEvent notification, CancellationToken cancellationToken)
        {
            var cancelledWithOutPay = new DTOs.Booking.BookingCancelledWithOutPay 
            { 
                BookingId = notification.BookingId,
                Reason = notification.Reason
            };
            await _integrationEventService.PublishAsync<DTOs.Booking.BookingCancelledWithOutPay>(
                cancelledWithOutPay,
                "booking.events",
                "topic",
                "booking.cancelled.notpay",
                cancellationToken);
        }
    }
}
