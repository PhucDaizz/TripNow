using PaymentService.Domain.Common;

namespace PaymentService.Domain.Events.RefundRequest
{
    public record RefundRequestCompletedEvent(Guid id, Guid useRefundId, decimal amountRefund): DomainEvent;
}
