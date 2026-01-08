using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Room
{
    public record RoomUnderMaintenanceEvent(Guid roomTypeId, DateOnly fromDate, DateOnly toDate): DomainEvent;
}
