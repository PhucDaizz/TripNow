using MediatR;

namespace Application.DTOs.User.Event
{
    public class UnfollowEvent: INotification
    {
        public Guid UserId { get; set; }
    }
}
