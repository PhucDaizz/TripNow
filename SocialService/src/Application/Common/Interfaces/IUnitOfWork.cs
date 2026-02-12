using SocialService.Domain.Repositories;

namespace SocialService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public ICommentRepository commentRepository { get; }
        public ILocationRepository locationRepository { get; }
        public IPostLikeRepository postLikeRepository { get; }
        public IPostRepository postRepository { get; }
        public ISavedPostRepository savedPostRepository { get; }
        public IUserFollowRepository userFollowRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
