using ChatService.Application.Features.Conversation.Commands.MarkAsRead;
using ChatService.Application.Features.Conversation.Commands.SendMessage;
using ChatService.Application.HubInterfaces;
using ChatService.Domain.Common;
using ChatService.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatService.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendMessage(SendMessageCommand command)
        {
            var userIdString = Context.UserIdentifier;
            if (Guid.TryParse(userIdString, out var userId))
            {
                command.CurrentUserId = userId;
            }

            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            command.CurrentUserRole = userRole switch
            {
                AppRoles.Customer => SenderType.Customer,

                AppRoles.Receptionist or AppRoles.HotelOwner or AppRoles.Housekeeping => SenderType.Hotel,

                AppRoles.SysAdmin => SenderType.SystemBot, 

                _ => SenderType.Customer
            };

            command.CurrentUserName = Context.User?.FindFirst(ClaimTypes.Name)?.Value
                           ?? Context.User?.FindFirst("name")?.Value
                           ?? "Người dùng ẩn danh";

            await _mediator.Send(command);
        }

        public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
        {
            var userIdString = Context.UserIdentifier;
            if (!Guid.TryParse(userIdString, out var userId)) return;

            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Khách hàng";

            string groupName = $"Conversation_{conversationId}";

            await Clients.OthersInGroup(groupName)
                     .ReceiveTypingIndicator(conversationId, userId, userName, isTyping);
        }

        public async Task MarkAsRead(Guid conversationId)
        {
            var userIdString = Context.UserIdentifier;
            if (!Guid.TryParse(userIdString, out var userId)) return;

            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var roleEnum = userRole switch
            {
                AppRoles.Customer => SenderType.Customer,
                AppRoles.Receptionist or AppRoles.HotelOwner or AppRoles.Housekeeping => SenderType.Hotel,
                AppRoles.SysAdmin => SenderType.SystemBot,
                _ => SenderType.Customer
            };

            var command = new MarkMessagesAsReadCommand
            {
                ConversationId = conversationId,
                CurrentUserId = userId,
                CurrentUserRole = roleEnum
            };

            await _mediator.Send(command);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var hotelId = httpContext?.Request.Query["hotelId"].ToString();

            if (!string.IsNullOrEmpty(hotelId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Hotel_{hotelId}");
            }

            var conversationId = httpContext?.Request.Query["conversationId"].ToString();
            if (!string.IsNullOrEmpty(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
