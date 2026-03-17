using PaymentService.Domain.Common;

namespace PaymentService.Domain.Events.Payout
{
    public record PayoutRejectedEvent: DomainEvent
    {
        public Guid PayoutId { get; set; }
        public Guid OwnerWalletId { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}
