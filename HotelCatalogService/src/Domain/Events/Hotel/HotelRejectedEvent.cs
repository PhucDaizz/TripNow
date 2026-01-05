using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelRejectedEvent(Guid OwnerId, string HotelName, string Reason) : DomainEvent;
    
}
