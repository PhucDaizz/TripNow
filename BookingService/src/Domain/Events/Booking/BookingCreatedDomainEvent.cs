using BookingService.Domain.Common;

namespace BookingService.Domain.Events.Booking
{
    public record BookingCreatedDomainEvent(Entities.Booking Booking): DomainEvent;
}
