using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.DTOs.SocialNotification;
using NotificationService.Application.DTOs.SocialNotification.Event;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SocialNotification.EventHandlers
{
    public class PostLikedIntegrationEventHandler : INotificationHandler<PostLikedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public PostLikedIntegrationEventHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(PostLikedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var existingNotif = await _unitOfWork.SocialNotification.GetExistingForAggregationAsync(
                notification.OwnerId,
                notification.SocialActionType,
                notification.ReferenceId.ToString(),
                cancellationToken);

            Domain.Entities.SocialNotification finalNotif;

            if (existingNotif != null)
            {
                existingNotif.AddInteraction(notification.LastActorId, notification.LastActorName);
                finalNotif = existingNotif;
            }

            else
            {
                var newNotif = new Domain.Entities.SocialNotification(
                    notification.OwnerId,
                    notification.SocialActionType,
                    notification.ReferenceId.ToString(),
                    notification.LastActorId,
                    notification.LastActorName);

                await _unitOfWork.SocialNotification.AddAsync(newNotif);
                finalNotif = newNotif;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var notifDto = SocialNotificationDto.FromEntity(finalNotif, notification.ActorAvatarUrl ?? notification.LastActorId.ToString());

            await _notificationService.SendSocialNotificationAsync(notification.OwnerId, notifDto);

            int unreadCount = await _unitOfWork.SocialNotification.CountUnreadByUserIdAsync(notification.OwnerId, cancellationToken);

            await _notificationService.UpdateSocialBadgeCountAsync(notification.OwnerId, unreadCount);

        }
    }
}
