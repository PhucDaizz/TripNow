using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomCheckedOut
{
    public class RoomCheckedOutEvent: INotification
    {
        public Guid RoomId { get; set; }
    }
}
