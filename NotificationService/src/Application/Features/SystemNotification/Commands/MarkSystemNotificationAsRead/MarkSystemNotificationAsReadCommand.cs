using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SystemNotification.Commands.MarkSystemNotificationAsRead
{
    public class MarkSystemNotificationAsReadCommand: IRequest<Result>
    {
        public Guid NotificationId { get; init; }
        public Guid UserId { get; init; }
    }
}
