using NotificationService.Application.DTOs.SocialNotification;
using NotificationService.Application.DTOs.SystemNotification;

namespace NotificationService.Application.HubInterfaces
{
    public interface INotificationClient
    {
        Task ReceiveSystemNotification(SystemNotificationDto notification);
        Task UpdateSystemBadgeCount(int unreadCount);

        Task ReceiveSocialNotification(SocialNotificationDto notification);
        Task UpdateSocialBadgeCount(int unreadCount);
    }
}
