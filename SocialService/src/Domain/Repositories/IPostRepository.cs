using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Post post, CancellationToken token = default);
        Task UpdateAsync(Post post, CancellationToken token = default);
        Task DeleteAsync(Post post, CancellationToken token = default);
        Task<bool> IsPostExisting(Guid postId, CancellationToken token = default);
    }
}
