using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Application.DTOs.UserFollow.Event;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.UserFollow;

namespace SocialService.Application.Features.UserFollow.EventHandlers
{
    public class UserFollowedEventHandler : INotificationHandler<UserFollowedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public UserFollowedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(UserFollowedEvent notification, CancellationToken cancellationToken)
        {
            var follower = await _unitOfWork.memberRepository.GetByIdAsync(notification.FollowerId);
            if (follower == null) return;

            await _integrationEventService.PublishAsync<FollowUserRequest>(
                new FollowUserRequest { UserId = notification.TargetId },
                "social.events",
                "topic",
                "increase.follow.user",
                cancellationToken);

            await _integrationEventService.PublishAsync<UserFollowedIntegrationEvent>(
                new UserFollowedIntegrationEvent
                {
                    OwnerId = notification.TargetId,
                    ReferenceId = notification.FollowerId,
                    SocialActionType = SocialActionType.Follow,
                    LastActorId = follower.Id,
                    LastActorName = follower.FullName ?? "Một người dùng",
                    ActorAvatarUrl = follower.AvatarUrl
                },
                "social.events",
                "topic",
                "new.user.follow", 
                cancellationToken);
        }
    }
}
