using ChatService.Application.DTOs.Message;
using ChatService.Application.HubInterfaces;
using ChatService.Application.Interface;
using ChatService.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.SignalR
{
    public class ChatNotificationService: IChatNotificationService
    {
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public ChatNotificationService(IHubContext<ChatHub, IChatClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToConversationAsync(Guid conversationId, Guid receiverId, MessageDto message)
        {
            string groupName = $"Conversation_{conversationId}";

            await _hubContext.Clients.Group(groupName).ReceiveNewMessage(message);

            await _hubContext.Clients.User(receiverId.ToString()).ReceiveChatNotification(message);
        }

        public async Task SendMessageToHotelStaffAsync(Guid hotelId, MessageDto message)
        {
            string groupName = $"Hotel_{hotelId}";
            await _hubContext.Clients.Group(groupName).ReceiveNewMessage(message);
        }

        public async Task SendReadReceiptAsync(Guid conversationId, Guid readerId)
        {
            string groupName = $"Conversation_{conversationId}";

            await _hubContext.Clients.Group(groupName)
                 .ReceiveReadReceipt(conversationId, readerId, DateTime.UtcNow);
        }
    }
}
