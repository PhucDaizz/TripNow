using MediatR;

namespace BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeDeletedEvent
{
    public class RoomTypeDeletedEvent: INotification
    {
        public Guid RoomTypeId { get; init; }
        public Guid HotelId { get; init; }
    }
}
