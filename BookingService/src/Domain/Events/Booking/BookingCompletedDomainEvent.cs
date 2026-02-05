using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record BookingCompletedDomainEvent(Guid bookingId, Guid hotelId, decimal amount): DomainEvent;
}
