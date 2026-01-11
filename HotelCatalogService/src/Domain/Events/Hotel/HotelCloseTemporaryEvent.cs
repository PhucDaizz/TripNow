using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelCloseTemporaryEvent(Guid HotelId,DateOnly FromDate, DateOnly? ToDate): DomainEvent;
}
