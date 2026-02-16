using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface IPostLikeRepository
    {
        Task<PostLike?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(PostLike postLike, CancellationToken token = default);
        Task UpdateAsync(PostLike postLike, CancellationToken token = default);
        Task DeleteAsync(PostLike postLike, CancellationToken token = default);
        Task<PostLike?> GetByUserIdAndPostAsync(Guid userId, Guid postId, CancellationToken token = default);
    }
}
