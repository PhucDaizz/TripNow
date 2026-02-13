using MediatR;

namespace Application.DTOs.User.Event
{
    public class IncreaseFollowEvent: INotification
    {
        public Guid UserId { get; set; }
    }
}
