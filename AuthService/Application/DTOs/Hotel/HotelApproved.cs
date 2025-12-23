using MediatR;

namespace Application.DTOs.Hotel
{
    public class HotelApproved : INotification
    {
        public Guid HotelId { get; init; } 
        public string HotelName { get; init; }
        public Guid OwnerId { get; init; } 
    }
}
