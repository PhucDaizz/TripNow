using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record RoomCleanedEvent(
        Guid HotelId,
        Guid RoomId,
        DateTime CleanedAt
    ) : DomainEvent;
    
}
