using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record RestorePromotionDomainEvent: DomainEvent
    {
        public Guid HotelId { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; }
    }
}
