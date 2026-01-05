using MediatR;

namespace Application.DTOs.Hotel
{
    public class SuspendHotelEvent: INotification
    {
        public Guid OwnerId { get; init; }
        public string HotelName { get; init; }
        public string Reason { get; init; }
    }
}
