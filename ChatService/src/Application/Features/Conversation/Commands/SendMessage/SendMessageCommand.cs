using ChatService.Application.DTOs.Message;
using ChatService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.SendMessage
{
    public class SendMessageCommand: IRequest<Result<MessageDto>>
    {
        public Guid ConversationId { get; set; }
        public Guid? HotelId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType MessageType { get; set; }


        public Guid CurrentUserId { get; set; }
        public SenderType CurrentUserRole { get; set; }

        public string CurrentUserName { get; set; } = string.Empty;
    }
}
