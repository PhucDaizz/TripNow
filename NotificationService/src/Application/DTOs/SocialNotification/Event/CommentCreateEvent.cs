namespace NotificationService.Application.DTOs.SocialNotification.Event
{
    public class CommentCreateEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
