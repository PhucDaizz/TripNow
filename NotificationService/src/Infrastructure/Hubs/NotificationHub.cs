using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Features.SocialNotification.Commands.MarkAllSocialNotificationsAsRead;
using NotificationService.Application.Features.SocialNotification.Commands.MarkSocialNotificationAsRead;
using NotificationService.Application.Features.SocialNotification.Queries.GetUnreadSystemCount;
using NotificationService.Application.Features.SystemNotification.Commands.MarkAllSystemNotificationsAsRead;
using NotificationService.Application.Features.SystemNotification.Commands.MarkSystemNotificationAsRead;
using NotificationService.Application.Features.SystemNotification.Queries.GetUnreadSocialCount;
using NotificationService.Application.HubInterfaces;

namespace NotificationService.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub: Hub<INotificationClient>
    {
        private readonly IMediator _mediator;

        public NotificationHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task MarkSystemNotificationAsRead(Guid notificationId)
        {
            var userIdString = Context.UserIdentifier;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var command = new MarkSystemNotificationAsReadCommand 
                {
                     NotificationId = notificationId,
                     UserId = userId 
                };  
                await _mediator.Send(command);
            }
        }

        public async Task MarkSocialNotificationAsRead(Guid notificationId)
        {
            var userIdString = Context.UserIdentifier;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var command = new MarkSocialNotificationAsReadCommand
                {
                    NotificationId = notificationId,
                    UserId = userId
                };
                await _mediator.Send(command);
            }
        }

        public async Task MarkAllSystemAsRead()
        {
            var userIdString = Context.UserIdentifier;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _mediator.Send(new MarkAllSystemNotificationsAsReadCommand(userId));
            }
        }

        public async Task MarkAllSocialAsRead()
        {
            var userIdString = Context.UserIdentifier;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _mediator.Send(new MarkAllSocialNotificationsAsReadCommand(userId));
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userIdString = Context.UserIdentifier;

            if (Guid.TryParse(userIdString, out var userId))
            {
                var systemBadgeCount = await _mediator.Send(new GetUnreadSystemCountQuery(userId));
                await Clients.Caller.UpdateSystemBadgeCount(systemBadgeCount.Value);

                var socialBadgeCount = await _mediator.Send(new GetUnreadSocialCountQuery(userId));
                await Clients.Caller.UpdateSocialBadgeCount(socialBadgeCount.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
