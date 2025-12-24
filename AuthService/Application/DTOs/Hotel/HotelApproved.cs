using MediatR;

namespace Application.DTOs.Hotel
{
    public class HotelApproved : INotification
    {
        public string HotelId { get; init; } 
        public string HotelName { get; init; }
        public string OwnerId { get; init; } 
    }
}
