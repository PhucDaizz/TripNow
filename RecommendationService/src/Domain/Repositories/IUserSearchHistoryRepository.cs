using RecommendationService.Domain.Entities;

namespace RecommendationService.Domain.Repositories
{
    public interface IUserSearchHistoryRepository
    {
        Task AddAsync(UserSearchHistory entity);
        Task<UserSearchHistory?> GetByIdAsync(Guid id);
        Task<IEnumerable<UserSearchHistory>> GetByUserIdAsync(Guid userId, int limit = 10);
    }
}
