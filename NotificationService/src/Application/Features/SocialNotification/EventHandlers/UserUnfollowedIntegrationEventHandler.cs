using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.DTOs.SocialNotification.Event;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SocialNotification.EventHandlers
{
    public class UserUnfollowedIntegrationEventHandler : INotificationHandler<UserUnfollowedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public UserUnfollowedIntegrationEventHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(UserUnfollowedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var existingNotif = await _unitOfWork.SocialNotification.GetExistingForAggregationAsync(
                notification.OwnerId,
                notification.SocialActionType,
                notification.ReferenceId.ToString(), 
                cancellationToken);

            if (existingNotif == null) return;

            
            bool shouldDelete = existingNotif.RemoveInteraction(notification.LastActorId);

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

            if (notification.IsHotelNotification)
            {
                await _notificationService.UpdateHotelSocialBadgeCountAsync(notification.OwnerId, unreadCount);
            }
            else
            {
                await _notificationService.UpdateSocialBadgeCountAsync(notification.OwnerId, unreadCount);
            }
        }
    }
}
