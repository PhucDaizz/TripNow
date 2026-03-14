using SocialService.Domain.Enum.NotificationService;

namespace SocialService.Application.DTOs.Common
{
    public class SystemNotificationCreateEvent
    {
        public Guid OwnerId { get; set; } 
        public string Title { get; set; } 
        public string Message { get; set; }
        public NotificationType Type { get; set; } 
        public string ReferenceId { get; set; }
        public bool IsHotelNotification { get; set; }
    }
}
