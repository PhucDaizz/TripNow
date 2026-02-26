using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Enum;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class UserFollowRepository : IUserFollowRepository
    {
        private readonly IApplicationDbContext _context;

        public UserFollowRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserFollow userFollow, CancellationToken token = default)
        {
            await _context.UserFollows.AddAsync(userFollow, token);
        }

        public Task DeleteAsync(UserFollow userFollow, CancellationToken token = default)
        {
            _context.UserFollows.Remove(userFollow);
            return Task.CompletedTask;
        }

        public async Task<UserFollow?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.UserFollows.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<UserFollow?> GetByUserAndTargetAsync(Guid userFollowId, Guid targetId, TypeFollow typeFollow, CancellationToken token = default)
        {
            return await _context.UserFollows.FirstOrDefaultAsync(x => x.TargetId == targetId && x.Type == typeFollow && x.FollowerId == userFollowId, token);
        }

        public Task<bool> IsExisting(Guid userFollowId, Guid targetId, TypeFollow typeFollow, CancellationToken token = default)
        {
            return _context.UserFollows.AnyAsync(x => x.TargetId == targetId && x.Type == typeFollow && x.FollowerId == userFollowId, token);
        }

        public Task UpdateAsync(UserFollow userFollow, CancellationToken token = default)
        {
            _context.UserFollows.Update(userFollow);
            return Task.CompletedTask;
        }
    }
}
