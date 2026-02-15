using MediatR;

namespace SocialService.Application.Features.Member.EventHandlers.MemberChangeImage
{
    public class MemberChangeImageEvent: INotification
    {
        public Guid UserId { get; set; }
        public string ImageUrl { get; set; }
    }
}
