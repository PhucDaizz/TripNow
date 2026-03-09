using NotificationService.Application.DTOs.SocialNotification;
using NotificationService.Application.DTOs.SystemNotification;

namespace NotificationService.Application.Services
{
    public interface INotificationService
    {
        // Hệ thống
        Task SendSystemNotificationAsync(Guid userId, SystemNotificationDto notification);
        Task UpdateSystemBadgeCountAsync(Guid userId, int unreadCount);

        // MXH
        Task SendSocialNotificationAsync(Guid userId, SocialNotificationDto notification);
        Task UpdateSocialBadgeCountAsync(Guid userId, int unreadCount);
    }
}
