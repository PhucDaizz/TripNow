using NotificationService.Domain.Enum;

namespace NotificationService.Application.DTOs.SystemNotification.Event
{
    public class SystemNotificationCreateEvent: MediatR.INotification
    {
        public Guid OwnerId { get; set; } 
        public string Title { get; set; } 
        public string Message { get; set; } 
        public NotificationType Type { get; set; } 
        public string ReferenceId { get; set; }
        public bool IsHotelNotification { get; set; }
    }
}
