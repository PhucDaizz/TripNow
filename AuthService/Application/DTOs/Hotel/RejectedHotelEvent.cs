using MediatR;

namespace Application.DTOs.Hotel
{
    public class RejectedHotelEvent: INotification
    {
        public Guid OwnerId { get; init; }
        public string HotelName { get; init; }
        public string Reason { get; init; }
    }
}
