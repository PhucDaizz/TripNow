using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.RefundRequest;
using PaymentService.Domain.Events.RefundRequest;

namespace PaymentService.Application.Features.RefundRequest.EventHandlers
{
    public class RefundRequestCompletedEventHandler : INotificationHandler<RefundRequestCompletedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RefundRequestCompletedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RefundRequestCompletedEvent notification, CancellationToken cancellationToken)
        {
            await _integrationEventService.PublishAsync<RefundRequestCompleted>(
                new RefundRequestCompleted
                {
                    RefundId = notification.id,
                    BookingId = notification.bookingId,
                    UserRefundId = notification.useRefundId,
                    AmountRefund = notification.amountRefund
                },
                "payment.events",
                "topic",
                "refundrequest.completed",
                cancellationToken
            );
        }
    }
}
