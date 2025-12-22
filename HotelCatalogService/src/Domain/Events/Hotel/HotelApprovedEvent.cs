using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelApprovedEvent(Guid HotelId, Guid OwnerId): DomainEvent;
}
