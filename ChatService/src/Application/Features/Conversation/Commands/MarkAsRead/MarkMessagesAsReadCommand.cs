using ChatService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.MarkAsRead
{
    public class MarkMessagesAsReadCommand : IRequest<Result<bool>>
    {
        public Guid ConversationId { get; set; }
        public Guid CurrentUserId { get; set; }
        public SenderType CurrentUserRole { get; set; }
    }
}
