namespace ChatService.Application.DTOs.Conversation
{
    public class ConversationListDto
    {
        public Guid Id { get; set; } 
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }

        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }

        public string LastMessage { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
