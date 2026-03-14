using SocialService.Application.DTOs.Common;

namespace SocialService.Application.DTOs.UserFollow.Event
{
    public class UserUnfollowedIntegrationEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
