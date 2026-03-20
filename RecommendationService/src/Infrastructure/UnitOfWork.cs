using Microsoft.EntityFrameworkCore.Storage;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Repositories;

namespace RecommendationService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserSearchHistoryRepository? UserSearchHistories { get; }
        public IUserViewedHotelRepository? UserViewedHotels { get; }

        public UnitOfWork(ApplicationDbContext context, IUserSearchHistoryRepository userSearchHistoryRepository, IUserViewedHotelRepository userViewedHotelRepository)
        {
            _context = context;
            UserSearchHistories = userSearchHistoryRepository;
            UserViewedHotels = userViewedHotelRepository;
        }

        public IUserSearchHistoryRepository UserSearchHistoryRepository { get; }
        public IUserViewedHotelRepository UserViewedHotelRepository { get; }

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
