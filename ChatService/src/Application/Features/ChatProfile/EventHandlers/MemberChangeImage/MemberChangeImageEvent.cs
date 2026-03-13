using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.MemberChangeImage
{
    public class MemberChangeImageEvent : INotification
    {
        public Guid UserId { get; set; }
        public string ImageUrl { get; set; }
    }
}
