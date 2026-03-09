using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.DTOs.SocialNotification;
using NotificationService.Application.DTOs.SystemNotification;
using NotificationService.Application.HubInterfaces;
using NotificationService.Application.Services;
using NotificationService.Infrastructure.Hubs;

namespace NotificationService.Infrastructure.SignalR
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public NotificationService(IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task SendSystemNotificationAsync(Guid userId, SystemNotificationDto notification)
        {
            await _hubContext.Clients.User(userId.ToString()).ReceiveSystemNotification(notification);
        }

        public async Task UpdateSystemBadgeCountAsync(Guid userId, int unreadCount)
        {
            await _hubContext.Clients.User(userId.ToString()).UpdateSystemBadgeCount(unreadCount);
        }


        public async Task SendSocialNotificationAsync(Guid userId, SocialNotificationDto notification)
        {
            await _hubContext.Clients.User(userId.ToString()).ReceiveSocialNotification(notification);
        }

        public async Task UpdateSocialBadgeCountAsync(Guid userId, int unreadCount)
        {
            await _hubContext.Clients.User(userId.ToString()).UpdateSocialBadgeCount(unreadCount);
        }
    }
}
