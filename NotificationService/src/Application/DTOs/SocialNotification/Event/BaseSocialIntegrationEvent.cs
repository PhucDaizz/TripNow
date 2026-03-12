using MediatR;
using NotificationService.Domain.Enum;

namespace NotificationService.Application.DTOs.SocialNotification.Event
{
    public abstract class BaseSocialIntegrationEvent : INotification
    {
        public Guid OwnerId { get; set; }

        public SocialActionType SocialActionType { get; set; }

        public Guid ReferenceId { get; set; }  

        public Guid LastActorId { get; set; }

        public string LastActorName { get; set; } = null!;

        public string? ActorAvatarUrl { get; set; }
    }
}
