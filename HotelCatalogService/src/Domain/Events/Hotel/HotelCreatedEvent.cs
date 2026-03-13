using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelCreatedEvent(Guid HotelId, string HotelName) : DomainEvent;
}
