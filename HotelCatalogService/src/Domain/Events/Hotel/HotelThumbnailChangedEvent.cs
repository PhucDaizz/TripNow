using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelThumbnailChangedEvent(Guid HotelId, string ImageUrl) : DomainEvent;
}
