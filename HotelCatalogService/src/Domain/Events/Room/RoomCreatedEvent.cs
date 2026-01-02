using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Room
{
    public record RoomCreatedEvent(Guid RoomTypeId): DomainEvent;
}
