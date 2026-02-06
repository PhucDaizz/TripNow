using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class OwnerWalletRepository : IOwnerWalletRepository
    {
        private readonly IApplicationDbContext _context;

        public OwnerWalletRepository(IApplicationDbContext context)
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

        public Task UpdateAsync(OwnerWallet ownerWallet, CancellationToken token = default)
        {
            _context.OwnerWallet.Update(ownerWallet);
            return Task.CompletedTask;
        }
    }
}
