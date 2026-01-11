using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelReopenEvent(Guid hotelId): DomainEvent;
}
