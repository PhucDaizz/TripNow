using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface ISavedPostRepository
    {
        Task<SavedPost?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(SavedPost savedPost, CancellationToken token = default);
        Task UpdateAsync(SavedPost savedPost, CancellationToken token = default);
        Task DeleteAsync(SavedPost savedPost, CancellationToken token = default);
    }
}
