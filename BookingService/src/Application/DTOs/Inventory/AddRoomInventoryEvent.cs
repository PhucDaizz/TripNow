using MediatR;

namespace BookingService.Application.DTOs.Inventory
{
    public record AddRoomInventoryEvent: INotification
    {
        public Guid RoomtypeId { get; init; }
    }
}
