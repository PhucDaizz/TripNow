using BookingService.Domain.Common;
using BookingService.Domain.Entities;

namespace BookingService.Domain.Events.Booking
{
    public record BookingCancelledDomainEvent: DomainEvent
    {
        public DateOnly Fromdate { get; set; }
        public DateOnly ToDate { get; set; }
        public List<BookingItem> Items { get; set; }
    }
}
