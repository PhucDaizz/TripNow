using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IApplicationDbContext _context;

        public CommentRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Comment comment, CancellationToken token = default)
        {
            await _context.Comments.AddAsync(comment, token);
        }

        public async Task DeleteAsync(Comment comment, CancellationToken token = default)
        {
            await _context.Comments.AddAsync(comment, token);
        }

        public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Comments.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(Comment comment, CancellationToken token = default)
        {
            _context.Comments.Update(comment);
            return Task.CompletedTask;
        }
    }
}
