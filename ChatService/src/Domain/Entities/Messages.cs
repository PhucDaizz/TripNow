using ChatService.Domain.Common;
using ChatService.Domain.Enum;
using ChatService.Domain.Exceptions;

namespace ChatService.Domain.Entities
{
    public class Messages: BaseEntity
    {
        public Guid ConversationId { get; private set; } 
        public Guid SenderId { get; private set; }
        public SenderType SenderType { get; private set; }
        public string Content { get; private set; }
        public MessageType MessageType { get; private set; }
        public bool IsRead { get; set; }

        protected Messages() { }

        private Messages(Guid conversationId, Guid senderId, SenderType senderType, string content, MessageType messageType)
        {
            ConversationId = conversationId;
            SenderId = senderId;
            SenderType = senderType;
            Content = content;
            MessageType = messageType;
            IsRead = false; 
        }

        public static Messages CreateMessage(Guid conversationId, Guid senderId, SenderType senderType, string content, MessageType messageType)
        {
            if(content == null) 
                throw new DomainException("Không được để tin nhắn rỗng");

            return new Messages(conversationId, senderId, senderType, content, messageType);
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
