using ChatService.Application.DTOs.Message;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Queries.GetChatHistory
{
    public class GetChatHistoryQuery : IRequest<Result<List<MessageDto>>>
    {
        public Guid ConversationId { get; set; }

        // Điểm neo thời gian: Lấy những tin nhắn xảy ra TRƯỚC thời điểm này
        // Nếu truyền null -> Mặc định là load lần đầu tiên (lấy 50 tin mới nhất hiện tại)
        public DateTime? Before { get; set; }

        // Mặc định mỗi lần kéo load 50 tin 
        public int Size { get; set; } = 50;
    }
}
