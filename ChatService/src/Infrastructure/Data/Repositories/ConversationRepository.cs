using ChatService.Application.Common.Interfaces;
using ChatService.Domain.Entities;
using ChatService.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Data.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IApplicationDbContext _context;

        public ConversationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Conversations conversation, CancellationToken token = default)
        {
            await _context.Conversations.AddAsync(conversation, token); 
        }

        public Task DeleteAsync(Conversations conversation, CancellationToken token = default)
        {
            _context.Conversations.Remove(conversation);
            return Task.CompletedTask;
        }

        public async Task<List<Conversations?>> GetAllConversation(Guid userId, CancellationToken token = default)
        {
            return await _context.Conversations
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync(token);
        }

        public Task<Conversations?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _context.Conversations.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<Conversations?> GetByUserIdAndHotelIdAsync(Guid userId, Guid hotelId, CancellationToken token = default)
        {
            return _context.Conversations.FirstOrDefaultAsync(x => x.UserId == userId && x.HotelId == hotelId, token);
        }

        public async Task<(List<Conversations> Items, int TotalCount)> GetConversationsByHotelIdAsync(
            Guid hotelId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            var query = _context.Conversations
                .Where(x => x.HotelId == hotelId)
                .Where(x => !string.IsNullOrEmpty(x.LastMessage));

            var totalCount = await query.CountAsync(token);

            var items = await query
                .OrderByDescending(x => x.UpdatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            if (totalCount == 0)
                return (new List<Conversations>(), 0);

            return (items, totalCount);
        }

        public async Task<(List<Conversations> Items, int TotalCount)> GetConversationsByUserIdAsync(
            Guid userId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            var query = _context.Conversations
                .Where(x => x.UserId == userId)
                .Where(x => !string.IsNullOrEmpty(x.LastMessage));

            var totalCount = await query.CountAsync(token);

            var items = await query
                .OrderByDescending(x => x.UpdatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            if (totalCount == 0)
                return (new List<Conversations>(), 0);

            return (items, totalCount); 
        }

        public async Task<List<Messages?>> GetUnreadMessage(Guid userReadId, Guid conversationId, CancellationToken token = default)
        {
            var unreadMessages = await _context.Conversations
                .Where(c => c.Id == conversationId)
                .SelectMany(c => c.MessageList)
                .Where(m => !m.IsRead && m.SenderId != userReadId)
                .ToListAsync(token);

            return unreadMessages;
        }
    }
}
