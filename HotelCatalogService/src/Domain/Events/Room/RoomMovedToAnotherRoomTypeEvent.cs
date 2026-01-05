using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Room
{
    public record RoomMovedToAnotherRoomTypeEvent(Guid oldRoomType, Guid newRoomType): DomainEvent;
}
