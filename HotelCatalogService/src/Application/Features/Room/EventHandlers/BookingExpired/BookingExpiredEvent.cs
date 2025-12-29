using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.BookingExpired
{
    public class BookingExpiredEvent : INotification 
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; } 
        public DateTime ExpiredAt { get; set; }
    }
}
