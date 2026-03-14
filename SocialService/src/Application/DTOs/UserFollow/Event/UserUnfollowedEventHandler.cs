using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Enum;
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
            bool isHotel = notification.Type == TypeFollow.FollowHotel;

            if (isHotel)
            {
                await _integrationEventService.PublishAsync<UnfollowHotelEvent>(
                    new UnfollowHotelEvent { HotelId = notification.TargetId },
                    "social.events",
                    "topic",
                    "unfollow.hotel",
                    cancellationToken
                );
            }
            else
            {
                await _integrationEventService.PublishAsync<UnfollowEvent>(
                    new UnfollowEvent { UserId = notification.TargetId },
                    "social.events",
                    "topic",
                    "unfollow.user",
                    cancellationToken
                );
            }


            await _integrationEventService.PublishAsync(
                new UserUnfollowedIntegrationEvent
                {
                    OwnerId = notification.TargetId,
                    SocialActionType = SocialActionType.Follow,
                    ReferenceId = notification.FollowerId, 
                    LastActorId = notification.FollowerId, 
                    LastActorName = string.Empty,
                    IsHotelNotification = isHotel
                },
                "social.events",
                "topic",
                "remove.user.follow",
                cancellationToken
            );
        }
    }
}
