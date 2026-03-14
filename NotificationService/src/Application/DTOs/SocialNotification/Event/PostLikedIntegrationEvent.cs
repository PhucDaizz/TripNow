namespace NotificationService.Application.DTOs.SocialNotification.Event
{
    public class PostLikedIntegrationEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
