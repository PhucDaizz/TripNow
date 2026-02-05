using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record BookingCancelledWithOutPayDomainEvent: DomainEvent
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
