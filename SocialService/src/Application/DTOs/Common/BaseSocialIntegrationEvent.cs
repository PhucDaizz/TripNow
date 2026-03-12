using SocialService.Domain.Enum.NotificationService;

namespace SocialService.Application.DTOs.Common
{
    public abstract class BaseSocialIntegrationEvent 
    {
        public Guid OwnerId { get; set; }

        public SocialActionType SocialActionType { get; set; }

        public Guid ReferenceId { get; set; }

        public Guid LastActorId { get; set; }

        public string LastActorName { get; set; } = null!;

        public string? ActorAvatarUrl { get; set; }
    }
}
