using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.UserFollow;

namespace SocialService.Application.DTOs.UserFollow.Event
{
    public class UserUnfollowedEventHandler : INotificationHandler<UserUnfollowedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public UserUnfollowedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(UserUnfollowedEvent notification, CancellationToken cancellationToken)
        {
            
            await _integrationEventService.PublishAsync<UnfollowEvent>(
                new UnfollowEvent
                {
                    UserId = notification.TargetId 
                },
                "social.events",
                "topic",
                "unfollow.user", 
                cancellationToken
            );

            
            await _integrationEventService.PublishAsync(
                new UserUnfollowedIntegrationEvent
                {
                    OwnerId = notification.TargetId,
                    SocialActionType = SocialActionType.Follow,
                    ReferenceId = notification.FollowerId, 
                    LastActorId = notification.FollowerId, 
                    LastActorName = string.Empty 
                },
                "social.events",
                "topic",
                "remove.user.follow",
                cancellationToken
            );
        }
    }
}
