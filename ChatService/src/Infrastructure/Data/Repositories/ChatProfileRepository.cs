using ChatService.Application.Common.Interfaces;
using ChatService.Domain.Entities;
using ChatService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Data.Repositories
{
    public class ChatProfileRepository : IChatProfileRepository
    {
        private readonly IApplicationDbContext _context;

        public ChatProfileRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatProfile chatProfile, CancellationToken token = default)
        {
            await _context.ChatProfiles.AddAsync(chatProfile, token);
        }

        public Task DeleteAsync(ChatProfile chatProfile, CancellationToken token = default)
        {
            _context.ChatProfiles.Remove(chatProfile);
            return Task.CompletedTask;
        }

        public async Task<ChatProfile?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.ChatProfiles.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<List<ChatProfile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.ChatProfiles
                         .AsNoTracking()
                         .Where(x => ids.Contains(x.Id))
                         .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsExistingAsync(Guid id, CancellationToken token = default)
        {
            return await _context.ChatProfiles.AnyAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(ChatProfile chatProfile, CancellationToken token = default)
        {
            _context.ChatProfiles.Update(chatProfile);
            return Task.CompletedTask;
        }
    }
}
