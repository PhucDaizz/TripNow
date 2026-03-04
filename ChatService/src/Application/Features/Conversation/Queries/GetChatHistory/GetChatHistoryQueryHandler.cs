using ChatService.Application.Common.Interfaces;
using ChatService.Application.DTOs.Message;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Features.Conversation.Queries.GetChatHistory
{
    public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, Result<List<MessageDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetChatHistoryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MessageDto>>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Messages.AsQueryable()
                .Where(m => m.ConversationId == request.ConversationId);

            if (request.Before.HasValue)
            {
                query = query.Where(m => m.CreatedAt < request.Before.Value);
            }

            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(request.Size)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderType = m.SenderType,
                    Content = m.Content,
                    MessageType = m.MessageType,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt,

                    SenderName = m.SenderType == Domain.Enum.SenderType.Customer ? "Khách hàng" : "Lễ tân"
                })
                .ToListAsync(cancellationToken);

            messages.Reverse();

            return Result.Success(messages);
        }
    }
}
