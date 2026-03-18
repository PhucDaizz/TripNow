using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Common.Interfaces;
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
        private readonly IHotelAuthorizationService _hotelAuthorizationService;

        public NotificationHub(IMediator mediator, IHotelAuthorizationService hotelAuthorizationService)
        {
            _mediator = mediator;
            _hotelAuthorizationService = hotelAuthorizationService;
        }

        public async Task MarkSystemNotificationAsRead(Guid notificationId, Guid? hotelId = null)
        {
            var (ownerId, isHotel) = await ResolveOwnerAndTypeAsync(hotelId);

            var command = new MarkSystemNotificationAsReadCommand
            {
                NotificationId = notificationId,
                OwnerId = ownerId,
                IsHotel = isHotel 
            };
            await _mediator.Send(command);
        }

        public async Task MarkSocialNotificationAsRead(Guid notificationId, Guid? hotelId = null)
        {
            var (ownerId, isHotel) = await ResolveOwnerAndTypeAsync(hotelId);

            var command = new MarkSocialNotificationAsReadCommand
            {
                NotificationId = notificationId,
                OwnerId = ownerId,
                IsHotel = isHotel
            };
            await _mediator.Send(command);
        }

        public async Task MarkAllSystemAsRead(Guid? hotelId = null)
        {
            var (ownerId, isHotel) = await ResolveOwnerAndTypeAsync(hotelId);

            await _mediator.Send(new MarkAllSystemNotificationsAsReadCommand
            {
                OwnerId = ownerId,
                IsHotel = isHotel
            });
        }

        public async Task MarkAllSocialAsRead(Guid? hotelId = null)
        {
            var (ownerId, isHotel) = await ResolveOwnerAndTypeAsync(hotelId);

            await _mediator.Send(new MarkAllSocialNotificationsAsReadCommand
            {
                OwnerId = ownerId,
                IsHotel = isHotel
            });
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

                // Lấy hotelId từ QueryString lúc connect
                var httpContext = Context.GetHttpContext();
                var hotelIdString = httpContext?.Request.Query["hotelId"].ToString();

                if (Guid.TryParse(hotelIdString, out var requestedHotelId))
                {
                    bool hasAccess = await _hotelAuthorizationService.HasHotelAccessAsync(requestedHotelId, Context.ConnectionAborted);

                    if (hasAccess)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"Hotel_{requestedHotelId}");

                        var hotelSocialBadgeCount = await _mediator.Send(new GetUnreadSocialCountQuery(requestedHotelId));
                        await Clients.Caller.UpdateSocialBadgeCount(hotelSocialBadgeCount.Value);

                        var hotelSystemBadgeCount = await _mediator.Send(new GetUnreadSystemCountQuery(requestedHotelId));
                        await Clients.Caller.UpdateSystemBadgeCount(hotelSystemBadgeCount.Value);
                    }
                    else
                    {
                        // CỐ TÌNH TRUYỀN ID BẬY -> ĐÁ VĂNG LUÔN HOẶC BỎ QUA
                        // Bác có thể ngắt kết nối luôn bằng lệnh dưới đây nếu muốn cực gắt:
                        // Context.Abort(); 
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<(Guid OwnerId, bool IsHotel)> ResolveOwnerAndTypeAsync(Guid? requestedHotelId)
        {
            var userId = Guid.Parse(Context.UserIdentifier!);

            if (requestedHotelId.HasValue && requestedHotelId.Value != Guid.Empty)
            {
                bool hasAccess = await _hotelAuthorizationService.HasHotelAccessAsync(requestedHotelId.Value, Context.ConnectionAborted);
                if (!hasAccess)
                {
                    throw new HubException("Bạn không có quyền thao tác trên dữ liệu của khách sạn này.");
                }
                return (requestedHotelId.Value, true);
            }

            return (userId, false);
        }
    }
}
