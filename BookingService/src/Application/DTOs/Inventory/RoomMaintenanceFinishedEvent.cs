using MediatR;

namespace BookingService.Application.DTOs.Inventory
{
    public class RoomMaintenanceFinishedEvent: INotification
    {
        public Guid RoomTypeId { get; init; }
        public DateOnly FromDate { get; init; } 
        public DateOnly ToDate { get; init; }
    }
}
