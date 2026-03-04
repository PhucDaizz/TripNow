using ChatService.Domain.Common;
using ChatService.Domain.Enum;

namespace ChatService.Domain.Entities
{
    public class Conversations: BaseEntity, AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid HotelId { get; private set; }
        public string? LastMessage { get; private set; }
        public int CustomerUnreadCount { get; private set; }
        public int HotelUnreadCount { get; private set; }

        private readonly List<Messages> _messages = new();
        public IReadOnlyCollection<Messages> MessageList => _messages.AsReadOnly();

        private Conversations() { }

        private Conversations(Guid userId, Guid hotelId)
        {
            UserId = userId;
            HotelId = hotelId;
            CustomerUnreadCount = 0;
            HotelUnreadCount = 0;
        }

        public static Conversations Create(Guid userId, Guid hotelId)
        {
            return new Conversations(userId, hotelId);
        }

        public void UpdateLastMessage(string content, SenderType senderType)
        {
            LastMessage = content;
            UpdatedAt = DateTime.UtcNow; 

            if (senderType == SenderType.Customer)
                HotelUnreadCount++;
            else if (senderType == SenderType.Hotel || senderType == SenderType.SystemBot)
                CustomerUnreadCount++;
        }

        public void AddMessage(Messages message)
        {
            _messages.Add(message);
        }

        public void ResetUnreadCount(SenderType readerType)
        {
            if (readerType == SenderType.Customer)
                CustomerUnreadCount = 0;
            else if (readerType == SenderType.Hotel)
                HotelUnreadCount = 0;
        }
    }
}
