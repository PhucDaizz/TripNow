using SocialService.Domain.Enum.NotificationService;

namespace SocialService.Application.DTOs.PostLike.Event
{
    public class PostUnlikedIntegrationEvent
    {
        public Guid OwnerId { get; set; }
        public SocialActionType SocialActionType { get; set; }
        public Guid ReferenceId { get; set; }
        public Guid UnlikedUserId { get; set; }
    }
}
