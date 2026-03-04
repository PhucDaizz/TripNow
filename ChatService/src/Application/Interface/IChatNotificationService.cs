using ChatService.Application.DTOs.Message;

namespace ChatService.Application.Interface
{
    public interface IChatNotificationService
    {
        Task SendMessageToConversationAsync(Guid conversationId, Guid receiverId, MessageDto message);
        Task SendMessageToHotelStaffAsync(Guid hotelId, MessageDto message);
        Task SendReadReceiptAsync(Guid conversationId, Guid readerId); 
    }
}
