using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Comment comment, CancellationToken token = default);
        Task UpdateAsync(Comment comment, CancellationToken token = default);
        Task DeleteAsync(Comment comment, CancellationToken token = default);
        Task<bool> IsParentCommentExistingAsync(Guid commentId, CancellationToken token = default);
    }
}
