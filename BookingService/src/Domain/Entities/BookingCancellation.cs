using BookingService.Domain.Common;
using BookingService.Domain.Enum;

namespace BookingService.Domain.Entities
{
    public class BookingCancellation : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public DateTime CancelledAt { get; private set; }
        public CancelledBy CancelledBy { get; private set; }
        public string Reason { get; private set; }

        public RefundPolicy RefundPolicyType { get; private set; }
        public decimal RefundAmount { get; private set; }
        private BookingCancellation() {}

        internal BookingCancellation(Guid bookingId, CancelledBy by, string reason, RefundPolicy policy, decimal amount)
        {
            BookingId = bookingId;
            CancelledBy = by;
            Reason = reason;
            RefundPolicyType = policy;
            RefundAmount = amount;
            CancelledAt = DateTime.UtcNow;
        }

    }
}
