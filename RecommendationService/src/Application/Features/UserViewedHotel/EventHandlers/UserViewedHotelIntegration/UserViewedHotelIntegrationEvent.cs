using MediatR;

namespace RecommendationService.Application.Features.UserViewedHotel.EventHandlers.UserViewedHotelIntegration
{
    public class UserViewedHotelIntegrationEvent: INotification
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
