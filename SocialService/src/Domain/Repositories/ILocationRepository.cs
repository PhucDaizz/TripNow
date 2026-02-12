using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Location location, CancellationToken token = default);
        Task UpdateAsync(Location location, CancellationToken token = default);
        Task DeleteAsync(Location location, CancellationToken token = default);
    }
}
