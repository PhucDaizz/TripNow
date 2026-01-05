using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Room
{
    public record RoomRemovedEvent(Guid RoomTypeId) : DomainEvent;
}
