using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface IUserFollowRepository
    {
        Task<UserFollow?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(UserFollow userFollow, CancellationToken token = default);
        Task UpdateAsync(UserFollow userFollow, CancellationToken token = default);
        Task DeleteAsync(UserFollow userFollow, CancellationToken token = default);
    }
}
