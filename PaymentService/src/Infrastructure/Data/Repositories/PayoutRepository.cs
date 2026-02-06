using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
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

        public async Task<PagedResult<Payout>> GetPagedListAsync(int pageNumber, int pageSize, PayoutStatus? status, Guid? ownerWalletId, string? searchTransactionRef, DateTime? fromDate, DateTime? toDate, CancellationToken token)
        {
            var query = _context.Payout.AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (ownerWalletId.HasValue)
            {
                query = query.Where(x => x.OwnerWalletId == ownerWalletId.Value);
            }

            if (!string.IsNullOrEmpty(searchTransactionRef))
            {
                query = query.Where(x => x.TransactionReference.Contains(searchTransactionRef));
            }

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                query = query.Where(x => x.CreatedAt >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedAt <= to);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(token);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedResult<Payout>(items, totalCount, pageNumber, pageSize);
        }

        public Task UpdateAsync(Payout payout, CancellationToken token = default)
        {
            _context.Payout.Update(payout);
            return Task.CompletedTask; throw new NotImplementedException();
        }
    }
}
