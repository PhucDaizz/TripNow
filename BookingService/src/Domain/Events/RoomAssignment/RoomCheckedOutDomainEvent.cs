using BookingService.Domain.Common;

namespace BookingService.Domain.Events.RoomAssignment
{
    public record RoomCheckedOutDomainEvent: DomainEvent
    {
        public Guid RoomId { get; set; }
    }
}
