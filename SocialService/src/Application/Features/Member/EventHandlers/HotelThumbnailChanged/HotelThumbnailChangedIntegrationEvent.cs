using MediatR;

namespace SocialService.Application.Features.Member.EventHandlers.HotelThumbnailChanged
{
    public class HotelThumbnailChangedIntegrationEvent: INotification
    {
        public Guid HotelId { get; set; }
        public string ImageUrl { get; set; }
    }
}
