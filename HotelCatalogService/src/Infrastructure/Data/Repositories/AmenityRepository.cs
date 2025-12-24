using HotelCatalogService.Domain.Entities;
using HotelCatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Infrastructure.Data.Repositories
{
    public class AmenityRepository: IAmenityRepository
    {
        private readonly ApplicationDbContext _context;

        public AmenityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Amenity.FindAsync(new object[] { id }, cancellationToken);

        public async Task<List<Amenity>> GetAllAsync(CancellationToken cancellationToken)
            => await _context.Amenity.AsNoTracking().ToListAsync(cancellationToken);

        public async Task AddAsync(Amenity amenity, CancellationToken cancellationToken)
            => await _context.Amenity.AddAsync(amenity, cancellationToken);

        public Task UpdateAsync(Amenity amenity, CancellationToken cancellationToken)
        {
            _context.Amenity.Update(amenity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Amenity amenity, CancellationToken cancellationToken)
        {
            _context.Amenity.Remove(amenity);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid amenityId, CancellationToken token)
        {
            var exists = await _context.Amenity.AnyAsync(a => a.Id == amenityId);
            return exists;
        }
    }
}
