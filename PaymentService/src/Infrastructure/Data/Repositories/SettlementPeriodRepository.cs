using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class SettlementPeriodRepository : ISettlementPeriodRepository
    {
        private readonly IApplicationDbContext _context;

        public SettlementPeriodRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SettlementPeriod settlementPeriod, CancellationToken token = default)
        {
            await _context.SettlementPeriod.AddAsync(settlementPeriod, token);
        }

        public Task DeleteAsync(SettlementPeriod settlementPeriod, CancellationToken token = default)
        {
            _context.SettlementPeriod.Remove(settlementPeriod);
            return Task.CompletedTask;
        }

        public async Task<SettlementPeriod?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.SettlementPeriod.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<SettlementPeriod?> GetByIdWithItemsAsync(Guid id, CancellationToken token = default)
        {
            return await _context.SettlementPeriod
                .Include(x => x.SettlementItems) 
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<SettlementPeriod>> GetPagedListAsync(Guid? ownerId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, CancellationToken token)
        {
            var query = _context.SettlementPeriod.AsNoTracking().AsQueryable();

            if (ownerId.HasValue)
                query = query.Where(x => x.OwnerId == ownerId.Value);

            if (fromDate.HasValue) query = query.Where(x => x.PeriodFrom >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.PeriodTo <= toDate.Value);

            query = query.OrderByDescending(x => x.PeriodFrom);

            var total = await query.CountAsync(token);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return new PagedResult<SettlementPeriod>(items, total, pageNumber, pageSize);
        }

        public Task UpdateAsync(SettlementPeriod settlementPeriod, CancellationToken token = default)
        {
            _context.SettlementPeriod.Update(settlementPeriod);
            return Task.CompletedTask;
        }
    }
}
