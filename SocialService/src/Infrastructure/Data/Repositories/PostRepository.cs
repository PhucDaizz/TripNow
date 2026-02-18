using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IApplicationDbContext _context;

        public PostRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Post post, CancellationToken token = default)
        {
            await _context.Posts.AddAsync(post, token);
        }

        public Task DeleteAsync(Post post, CancellationToken token = default)
        {
            _context.Posts.Remove(post);
            return Task.CompletedTask;
        }

        public async Task<Post?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Posts.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<bool> IsPostExisting(Guid postId, CancellationToken token = default)
        {
            return _context.Posts.AnyAsync(x => x.Id == postId && x.IsDeleted == false, token);
        }

        public Task UpdateAsync(Post post, CancellationToken token = default)
        {
            _context.Posts.Update(post);
            return Task.CompletedTask;
        }

        public async Task<Post?> GetPostDetailAsync(Guid postId, CancellationToken token = default)
        {
            var post = await _context.Posts
                .Include(x => x.Images)
                .Include(x => x.ReviewDetail)
                .FirstOrDefaultAsync(x => x.Id == postId);

            return post;
        }
    }
}
