using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class PayoutRepository : IPayoutRepository
    {
        private readonly ApplicationDbContext _context;

        public PayoutRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payout payout, CancellationToken token = default)
        {
            await _context.Payout.AddAsync(payout, token);
        }

        public Task DeleteAsync(Payout payout, CancellationToken token = default)
        {
            _context.Payout.Remove(payout);
            return Task.CompletedTask;
        }

        public async Task<Payout?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Payout.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<Payout?> GetBySettlementAsync(Guid settlementId, CancellationToken token = default)
        {
            return await _context.Payout.FirstOrDefaultAsync(x => x.SettlementId == settlementId, token);
        }

        public Task UpdateAsync(Payout payout, CancellationToken token = default)
        {
            _context.Payout.Update(payout);
            return Task.CompletedTask; throw new NotImplementedException();
        }
    }
}
