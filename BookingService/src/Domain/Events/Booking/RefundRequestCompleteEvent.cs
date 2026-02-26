using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record RefundRequestCompleteEvent: DomainEvent
    {
        public Guid RefundId { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserRefundId { get; set; }
        public decimal AmountRefund { get; set; }
    }
}
