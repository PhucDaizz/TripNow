using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.CreateOrGetConversation
{
    public class CreateOrGetConversationCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public Guid CurrentUserId { get; set; }
    }
}
