using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly IApplicationDbContext _context;

        public PostLikeRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PostLike postLike, CancellationToken token = default)
        {
            await _context.PostLikes.AddAsync(postLike, token);
        }

        public Task DeleteAsync(PostLike postLike, CancellationToken token = default)
        {
            _context.PostLikes.Remove(postLike);
            return Task.CompletedTask;
        }

        public async Task<PostLike?> GetByUserIdAndPostAsync(Guid userId, Guid postId, CancellationToken token = default)
        {
            return await _context.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId, token);
        }

        public async Task<PostLike?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.PostLikes.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(PostLike postLike, CancellationToken token = default)
        {
            _context.PostLikes.Update(postLike);
            return Task.CompletedTask;
        }
    }
}
