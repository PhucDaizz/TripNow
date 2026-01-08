using MediatR;

namespace BookingService.Application.DTOs.Inventory
{
    public record RoomMovedToAnotherRoomTypeEvent: INotification
    {
        public Guid OldRoomType { get; init; }
        public Guid NewRoomType { get; init; }
    }
}
