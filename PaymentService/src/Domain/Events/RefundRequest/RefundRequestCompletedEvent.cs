using PaymentService.Domain.Common;

namespace PaymentService.Domain.Events.RefundRequest
{
    public record RefundRequestCompletedEvent(Guid id, Guid bookingId, Guid useRefundId, decimal amountRefund): DomainEvent;
}
