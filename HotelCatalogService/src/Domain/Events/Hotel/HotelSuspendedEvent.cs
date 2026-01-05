using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelSuspendedEvent(Guid OwnerId, string HotelName, string Reason): DomainEvent;
}
