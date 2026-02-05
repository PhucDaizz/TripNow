using PaymentService.Domain.Common;

namespace PaymentService.Domain.Events.PaymentTransaction
{
    public record PaymentSucceededEvent(Guid BookingId, decimal Amount, decimal ProviderFee) : DomainEvent;
}
