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
            var conversation = await _context.ConversationsQuery
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken);

            if (conversation == null)
            {
                return Result.Failure<List<MessageDto>>(new Error("NOT.FOUND", "Conversation not found."));
            }

            var targetIds = new List<Guid> { conversation.UserId, conversation.HotelId };

            var profiles = await _context.ChatProfilesQuery
                .AsNoTracking()
                .Where(p => targetIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            profiles.TryGetValue(conversation.UserId, out var customerProfile);
            profiles.TryGetValue(conversation.HotelId, out var hotelProfile);

            var query = _context.MessagesQuery
                .AsNoTracking()
                .Where(m => m.ConversationId == request.ConversationId);

            if (request.Before.HasValue)
            {
                query = query.Where(m => m.CreatedAt < request.Before.Value);
            }

            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(request.Size)
                .ToListAsync(cancellationToken); 

            
            messages.Reverse(); 

            var resultDtos = messages.Select(m =>
            {
                string senderName = "Người dùng ẩn danh";
                string senderAvatar = "";

                if (m.SenderType == Domain.Enum.SenderType.Customer)
                {
                    senderName = customerProfile?.FullName ?? "Khách hàng";
                    senderAvatar = customerProfile?.AvatarUrl ?? "https://ui-avatars.com/api/?name=Customer&background=random";
                }
                else if (m.SenderType == Domain.Enum.SenderType.Hotel)
                {
                    senderName = hotelProfile?.FullName ?? "Khách sạn";
                    senderAvatar = hotelProfile?.AvatarUrl ?? "https://ui-avatars.com/api/?name=Hotel&background=random";
                }
                else if (m.SenderType == Domain.Enum.SenderType.SystemBot)
                {
                    senderName = "Hệ thống";
                    senderAvatar = "https://ui-avatars.com/api/?name=System&background=1D4ED8&color=fff";
                }

                return new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderType = m.SenderType,
                    Content = m.Content,
                    MessageType = m.MessageType,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt,
                    SenderName = senderName,
                    SenderAvatarUrl = senderAvatar 
                };
            }).ToList();

            return Result.Success(resultDtos);
        }
    }
}