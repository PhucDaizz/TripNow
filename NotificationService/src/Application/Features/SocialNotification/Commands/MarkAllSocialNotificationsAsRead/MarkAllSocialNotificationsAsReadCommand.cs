using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SocialNotification.Commands.MarkAllSocialNotificationsAsRead
{
    public record MarkAllSocialNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
}
