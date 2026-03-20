using RecommendationService.Domain.Repositories;

namespace RecommendationService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserSearchHistoryRepository UserSearchHistories { get; }
        IUserViewedHotelRepository UserViewedHotels { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
