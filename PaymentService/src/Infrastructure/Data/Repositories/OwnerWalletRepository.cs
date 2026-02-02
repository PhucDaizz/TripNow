using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
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

        public async Task<OwnerWallet?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.OwnerWallet.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(OwnerWallet ownerWallet, CancellationToken token = default)
        {
            _context.OwnerWallet.Update(ownerWallet);
            return Task.CompletedTask;
        }
    }
}
