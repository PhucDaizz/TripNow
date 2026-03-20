using Microsoft.EntityFrameworkCore;
using RecommendationService.Domain.Entities;
namespace RecommendationService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<UserViewedHotel> UserViewedHotels { get; }
        public DbSet<UserSearchHistory> UserSearchHistories { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
