namespace NotificationService.Application.DTOs.SocialNotification.Event
{
    public class UserFollowedIntegrationEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
