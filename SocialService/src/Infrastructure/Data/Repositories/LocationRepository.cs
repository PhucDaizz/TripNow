using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IApplicationDbContext _context;

        public LocationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Location location, CancellationToken token = default)
        {
            await _context.Locations.AddAsync(location, token);
        }

        public Task DeleteAsync(Location location, CancellationToken token = default)
        {
            _context.Locations.Remove(location);
            return Task.CompletedTask;
        }

        public async Task<Location?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(Location location, CancellationToken token = default)
        {
            _context.Locations.Update(location);
            return Task.CompletedTask;
        }
    }
}
