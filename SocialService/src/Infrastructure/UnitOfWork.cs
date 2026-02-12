using Microsoft.EntityFrameworkCore.Storage;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public ICommentRepository commentRepository { get; }
        public ILocationRepository locationRepository { get; }
        public IPostLikeRepository postLikeRepository { get; }
        public IPostRepository postRepository { get; }
        public ISavedPostRepository savedPostRepository { get; }
        public IUserFollowRepository userFollowRepository { get; }

        public UnitOfWork(ApplicationDbContext context,
            ICommentRepository commentRepository,
            ILocationRepository locationRepository,
            IPostLikeRepository postLikeRepository,
            IPostRepository postRepository,
            ISavedPostRepository savedPostRepository,
            IUserFollowRepository userFollowRepository)
        {
            _context = context;
            this.commentRepository = commentRepository;
            this.locationRepository = locationRepository;
            this.postLikeRepository = postLikeRepository;
            this.postRepository = postRepository;
            this.savedPostRepository = savedPostRepository;
            this.userFollowRepository = userFollowRepository;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
