using MediatR;

namespace BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeCreated
{
    public class RoomTypeCreatedEvent: INotification
    {
        public Guid RoomTypeId { get; init; }
        public Guid HotelId { get; init; }
        public int InitialStock { get; init; }
    }
}
