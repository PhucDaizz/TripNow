using SocialService.Application.DTOs.Common;
namespace SocialService.Application.DTOs.PostLike.Event
{
    public class PostLikedIntegrationEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
