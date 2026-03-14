using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SystemNotification.Commands.MarkAllSystemNotificationsAsRead
{
    public record MarkAllSystemNotificationsAsReadCommand: IRequest<Result>
    {
        public Guid OwnerId { get; set; }
        public bool IsHotel { get; set; }
    }
}
