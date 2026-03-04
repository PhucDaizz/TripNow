using ChatService.Application.DTOs.Conversation;
using ChatService.Domain.Common.Models;
using ChatService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Queries.GetAll
{
    public class GetAllConversationsQuery : IRequest<Result<PagedResult<ConversationListDto?>>>
    {
        public Guid CurrentUserId { get; set; }
        public Guid? HotelId { get; set; }
        public SenderType CurrentUserRole { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
