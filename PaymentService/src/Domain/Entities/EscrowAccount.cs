using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;

namespace PaymentService.Domain.Entities
{
    public class EscrowAccount: BaseEntity, AggregateRoot
    {
        public Guid BookingId { get; private set; }
        public decimal Amount { get; private set; }
        public EscrowStatus Status { get; private set; }
        public decimal ProviderFee { get; private set; }

        private EscrowAccount() { }
    }
}
