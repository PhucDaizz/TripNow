using ChatService.Application.DTOs.Message;

namespace ChatService.Application.HubInterfaces
{
    public interface IChatClient
    {
        Task ReceiveNewMessage(MessageDto message);
        Task ReceiveChatNotification(MessageDto message); // khi có tin nhắn mới, gửi thông báo đến người dùng nếu người dùng ơ trang chủ
        Task ReceiveTypingIndicator(Guid conversationId, Guid userId, string userName, bool isTyping); // áp dụng kỹ thuật Debounce / Throttle
        Task ReceiveReadReceipt(Guid conversationId, Guid readerId, DateTime readAt); // readerId là người đã đọc tin nhắn,
                                                                                      // gửi thông báo đến những người khác trong cuộc
                                                                                      // trò chuyện để cập nhật trạng thái đã đọc
                                                                                      // Kiểm tra xem người đọc CÓ PHẢI LÀ MÌNH KHÔNG?
                                                                                      // Lờ đi, không làm gì cả vì đây là tiếng vọng của chính mình
    }
}
