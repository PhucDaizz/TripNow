using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SystemNotification.Commands.MarkAllSystemNotificationsAsRead
{
    public record MarkAllSystemNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
}
