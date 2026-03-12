using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SocialNotification.EventHandlers
{
    public class PostUnlikedIntegrationEventHandler : INotificationHandler<PostUnlikedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public PostUnlikedIntegrationEventHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(PostUnlikedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var existingNotif = await _unitOfWork.SocialNotification.GetExistingForAggregationAsync(
                notification.OwnerId,
                notification.SocialActionType,
                notification.ReferenceId.ToString(),
                cancellationToken);

            if (existingNotif == null) return;

            bool shouldDelete = existingNotif.RemoveInteraction(notification.UnlikedUserId);

            if (shouldDelete)
            {
                await _unitOfWork.SocialNotification.RemoveAsync(existingNotif);
            }
            else
            {
                await _unitOfWork.SocialNotification.UpdateAsync(existingNotif);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

           
            int unreadCount = await _unitOfWork.SocialNotification.CountUnreadByUserIdAsync(notification.OwnerId, cancellationToken);

            await _notificationService.UpdateSocialBadgeCountAsync(notification.OwnerId, unreadCount);
        }
    }
}
