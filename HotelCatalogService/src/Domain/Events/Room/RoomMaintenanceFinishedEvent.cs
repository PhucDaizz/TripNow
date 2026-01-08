using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Room
{
    public record RoomMaintenanceFinishedEvent(Guid roomTypeId, DateOnly oldStart, DateOnly oldEnd) : DomainEvent;
    
}
