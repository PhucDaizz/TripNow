using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class OwnerWalletRepository : IOwnerWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerWalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OwnerWallet ownerWallet, CancellationToken token = default)
        {
            await _context.OwnerWallet.AddAsync(ownerWallet, token);
        }

        public Task DeleteAsync(OwnerWallet ownerWallet, CancellationToken token = default)
        {
            _context.OwnerWallet.Remove(ownerWallet);
            return Task.CompletedTask;
        }

        public async Task<List<Guid>> GetAllActiveOwnerIdsAsync(CancellationToken token)
        {
            return await _context.OwnerWallet
                .AsNoTracking() 
                .Where(x => x.PendingBalance > 0)
                .Select(x => x.OwnerId)
                .Distinct()
                .ToListAsync(token);
        }

        public async Task<OwnerWallet?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.OwnerWallet.FirstOrDefaultAsync(x => x.Id == id, token);
        }
        public async Task<OwnerWallet?> GetByOwnerIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.OwnerWallet.FirstOrDefaultAsync(x => x.OwnerId == id, token);
        }

        public async Task<PagedResult<WalletLedger>> GetPagedListAsync(Guid ownerId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? type, CancellationToken token)
        {
            var query = from ledger in _context.WalletLedger
                        join wallet in _context.OwnerWallet on ledger.WalletId equals wallet.Id
                        where wallet.OwnerId == ownerId
                        select ledger;

            if (fromDate.HasValue) query = query.Where(x => x.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.CreatedAt <= toDate.Value);

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<LedgerReferenceType>(type, out var enumType))
            {
                query = query.Where(x => x.ReferenceType == enumType);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var total = await query.CountAsync(token);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return new PagedResult<WalletLedger>(items, total, pageNumber, pageSize);
        }

        public async Task<List<WalletLedger>> GetPendingLedgersForSettlementAsync(Guid ownerId, DateTime cutOffDate, CancellationToken token)
        {
            return await _context.OwnerWallet
                .Where(w => w.OwnerId == ownerId) 
                .SelectMany(w => w.WalletLedgers) 
                .Where(l => l.SettlementPeriodId == null
                         && l.ReferenceType == LedgerReferenceType.Booking
                         && l.CreatedAt <= cutOffDate)
                .ToListAsync(token);
        }

        public Task<OwnerWallet?> GetWalletWithPendingLedgersAsync(Guid ownerId, DateTime cutOffDate, CancellationToken token)
        {
            return _context.OwnerWallet
                .Include(w => w.WalletLedgers.Where(l => l.SettlementPeriodId == null
                                                        && l.ReferenceType == LedgerReferenceType.Booking
                                                        && l.CreatedAt <= cutOffDate))
                .FirstOrDefaultAsync(w => w.OwnerId == ownerId, token);
        }

        public async Task<bool> HasTransactionAsync(Guid ownerId, Guid referenceId, LedgerReferenceType type, CancellationToken cancellationToken = default)
        {
            return await _context.OwnerWallet
                .Where(w => w.Id == ownerId)
                .SelectMany(w => w.WalletLedgers)
                .AnyAsync(l => l.ReferenceId == referenceId && l.ReferenceType == type, cancellationToken);
        }

        public Task UpdateAsync(OwnerWallet ownerWallet, CancellationToken token = default)
        {
            _context.OwnerWallet.Update(ownerWallet);
            return Task.CompletedTask;
        }
    }
}
