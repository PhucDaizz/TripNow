using PaymentService.Domain.Common;

namespace PaymentService.Domain.Events.EscrowAccount
{
    public record EscrowCreatedEvent(Guid BookingId):DomainEvent;
}
