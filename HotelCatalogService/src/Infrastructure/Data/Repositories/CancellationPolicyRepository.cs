using HotelCatalogService.Domain.Entities;
using HotelCatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Infrastructure.Data.Repositories
{
    public class CancellationPolicyRepository : ICancellationPolicyRepository
    {
        private readonly ApplicationDbContext _context;

        public CancellationPolicyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CancellationPolicy policy, CancellationToken cancellationToken = default)
        {
            await _context.CancellationPolicy.AddAsync(policy, cancellationToken);
        }

        public Task UpdateAsync(CancellationPolicy policy, CancellationToken cancellationToken = default)
        {
            _context.CancellationPolicy.Update(policy);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationPolicy policy, CancellationToken cancellationToken = default)
        {
            _context.CancellationPolicy.Remove(policy);
            return Task.CompletedTask;
        }

        public async Task<CancellationPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.CancellationPolicy
                .Include(p => p.Rules)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<CancellationPolicy>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.CancellationPolicy
                .Where(p => p.HotelId == hotelId)
                .Include(p => p.Rules)
                .ToListAsync(cancellationToken);
        }

        public async Task<CancellationPolicy?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.CancellationPolicy
                .Include(p => p.Rules)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
