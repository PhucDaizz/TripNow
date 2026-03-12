using SocialService.Application.DTOs.Common;

namespace SocialService.Application.DTOs.Comment.Event
{
    public class CommentCreateEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
