using MediatR;

namespace BookingService.Application.DTOs.Inventory
{
    public record DeductRoomInventoryEvent: INotification
    {
        public Guid RoomTypeId { get; init; }
    }
}
