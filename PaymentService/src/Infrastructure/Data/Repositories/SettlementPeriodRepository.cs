using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
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

        public Task UpdateAsync(SettlementPeriod settlementPeriod, CancellationToken token = default)
        {
            _context.SettlementPeriod.Update(settlementPeriod);
            return Task.CompletedTask;
        }
    }
}
