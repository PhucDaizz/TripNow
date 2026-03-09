using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SocialNotification.Commands.MarkSocialNotificationAsRead
{
    public class MarkSocialNotificationAsReadCommand : IRequest<Result>
    {
        public Guid NotificationId { get; init; }
        public Guid UserId { get; init; }
    }
}
