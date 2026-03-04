using ChatService.Domain.Enum;

namespace ChatService.Application.DTOs.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }

        public Guid SenderId { get; set; }
        public SenderType SenderType { get; set; }

        public string SenderName { get; set; }
        public string? SenderAvatarUrl { get; set; }

        public string Content { get; set; }
        public MessageType MessageType { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
