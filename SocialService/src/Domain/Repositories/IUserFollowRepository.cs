using SocialService.Domain.Entities;
using SocialService.Domain.Enum;

namespace SocialService.Domain.Repositories
{
    public interface IUserFollowRepository
    {
        Task<UserFollow?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<bool> IsExisting(Guid userFollowId, Guid targetId, TypeFollow typeFollow, CancellationToken token = default);
        Task<UserFollow?> GetByUserAndTargetAsync(Guid userFollowId, Guid targetId, TypeFollow typeFollow, CancellationToken token = default);
        Task AddAsync(UserFollow userFollow, CancellationToken token = default);
        Task UpdateAsync(UserFollow userFollow, CancellationToken token = default);
        Task DeleteAsync(UserFollow userFollow, CancellationToken token = default);
    }
}
