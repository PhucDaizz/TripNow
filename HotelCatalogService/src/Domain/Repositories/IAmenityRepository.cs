using HotelCatalogService.Domain.Entities;

namespace HotelCatalogService.Domain.Repositories
{
    public interface IAmenityRepository
    {
        Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Amenity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Amenity amenity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Amenity amenity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Amenity amenity, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid amenityId, CancellationToken token);
    }
}
