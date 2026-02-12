using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class SavedPostRepository : ISavedPostRepository
    {
        private readonly IApplicationDbContext _context;

        public SavedPostRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SavedPost savedPost, CancellationToken token = default)
        {
            await _context.SavedPosts.AddAsync(savedPost, token);
        }

        public async Task DeleteAsync(SavedPost savedPost, CancellationToken token = default)
        {
            await _context.SavedPosts.AddAsync(savedPost, token);
        }

        public async Task<SavedPost?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.SavedPosts.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(SavedPost savedPost, CancellationToken token = default)
        {
            _context.SavedPosts.Update(savedPost);
            return Task.CompletedTask;
        }
    }
}
