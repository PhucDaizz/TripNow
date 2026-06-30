using RecommendationService.Domain.Entities;

namespace RecommendationService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<UserViewedHotel> UserViewedHotelsQuery { get; }
        IQueryable<UserSearchHistory> UserSearchHistoriesQuery { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
