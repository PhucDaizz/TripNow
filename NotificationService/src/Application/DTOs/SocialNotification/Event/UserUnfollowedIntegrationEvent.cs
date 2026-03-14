namespace NotificationService.Application.DTOs.SocialNotification.Event
{
    public class UserUnfollowedIntegrationEvent: BaseSocialIntegrationEvent
    {
        public bool IsHotelNotification { get; set; }
    }
}
