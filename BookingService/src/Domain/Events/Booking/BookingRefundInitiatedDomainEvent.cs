using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record BookingRefundInitiatedDomainEvent : DomainEvent
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }

    };
}
