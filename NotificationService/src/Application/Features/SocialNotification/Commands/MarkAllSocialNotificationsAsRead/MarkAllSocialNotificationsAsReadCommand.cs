using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SocialNotification.Commands.MarkAllSocialNotificationsAsRead
{
    public record MarkAllSocialNotificationsAsReadCommand: IRequest<Result>
    {
        public Guid OwnerId { get; set; }
        public bool IsHotel { get; set; }
    }
}
