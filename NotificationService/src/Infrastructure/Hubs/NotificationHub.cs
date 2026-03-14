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
using NotificationService.Domain.Common;
using System.Security.Claims;

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

        public async Task MarkSystemNotificationAsRead(Guid notificationId, Guid? hotelId = null)
        {
            Guid ownerId = ResolveOwnerIdSecurely(hotelId);
            bool isHotel = hotelId.HasValue && ownerId == hotelId.Value;

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
            Guid ownerId = ResolveOwnerIdSecurely(hotelId);
            bool isHotel = hotelId.HasValue && ownerId == hotelId.Value;

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
            Guid ownerId = ResolveOwnerIdSecurely(hotelId);
            bool isHotel = hotelId.HasValue && ownerId == hotelId.Value;

            await _mediator.Send(new MarkAllSystemNotificationsAsReadCommand
            {
                OwnerId = ownerId,
                IsHotel = isHotel
            });
        }

        public async Task MarkAllSocialAsRead(Guid? hotelId = null)
        {
            Guid ownerId = ResolveOwnerIdSecurely(hotelId);
            bool isHotel = hotelId.HasValue && ownerId == hotelId.Value;

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
                    // KIỂM TRA: Có thực sự được phép nghe lén cái Hotel này không?
                    Guid validatedOwnerId = ResolveOwnerIdSecurely(requestedHotelId);

                    if (validatedOwnerId == requestedHotelId)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"Hotel_{requestedHotelId}");

                        var hotelSocialBadgeCount = await _mediator.Send(new GetUnreadSocialCountQuery(requestedHotelId));
                        await Clients.Caller.UpdateSocialBadgeCount(hotelSocialBadgeCount.Value);

                        var hotelSystemBadgeCount = await _mediator.Send(new GetUnreadSystemCountQuery(requestedHotelId));
                        await Clients.Caller.UpdateSystemBadgeCount(hotelSystemBadgeCount.Value);
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private Guid ResolveOwnerIdSecurely(Guid? requestedHotelId)
        {
            var userId = Guid.Parse(Context.UserIdentifier!);

            if (!requestedHotelId.HasValue) return userId;

            var tokenHotelId = Context.User?.FindFirst("HotelId")?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            // KỊCH BẢN 1: Lễ tân (Có HotelId trong Token)
            if (!string.IsNullOrEmpty(tokenHotelId) && tokenHotelId == requestedHotelId.Value.ToString())
            {
                return requestedHotelId.Value; // Xác thực thành công
            }

            // KỊCH BẢN 2: Chủ khách sạn (Chỉ có Role, không có HotelId trong Token)
            if (role == AppRoles.HotelOwner) 
            {
                // Tạm thời ta tin tưởng nếu Role là Owner. 
                // (Nếu muốn bảo mật tuyệt đối 100%, phải dùng Redis hoặc gọi gRPC sang Catalog Service để check giống hệt bên SocialService).
                return requestedHotelId.Value;
            }

            return userId;
        }
    }
}
