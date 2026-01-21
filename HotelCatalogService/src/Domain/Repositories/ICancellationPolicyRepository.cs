using HotelCatalogService.Domain.Entities;
using System.Linq.Expressions;

namespace HotelCatalogService.Domain.Repositories
{
    public interface ICancellationPolicyRepository
    {
        Task AddAsync(CancellationPolicy policy, CancellationToken cancellationToken = default);
        Task UpdateAsync(CancellationPolicy policy, CancellationToken cancellationToken = default);
        Task DeleteAsync(CancellationPolicy policy, CancellationToken cancellationToken = default);
        Task<CancellationPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CancellationPolicy>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
        Task<CancellationPolicy?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
