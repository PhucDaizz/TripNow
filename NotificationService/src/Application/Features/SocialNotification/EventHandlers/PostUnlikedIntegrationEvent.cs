using MediatR;
using NotificationService.Domain.Enum;

namespace NotificationService.Application.Features.SocialNotification.EventHandlers
{
    public class PostUnlikedIntegrationEvent: INotification
    {
        public Guid OwnerId { get; set; }
        public SocialActionType SocialActionType { get; set; }
        public Guid ReferenceId { get; set; }
        public Guid UnlikedUserId { get; set; }
    }
}
