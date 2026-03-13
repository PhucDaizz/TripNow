using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.HotelThumbnailChanged
{
    public class HotelThumbnailChangedIntegrationEvent : INotification
    {
        public Guid HotelId { get; set; }
        public string ImageUrl { get; set; }
    }
}
