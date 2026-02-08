using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class OwnerBankAccountRepository : IOwnerBankAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerBankAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default)
        {
            await _context.OwnerBankAccount.AddAsync(ownerBankAccount, token);
        }

        public async Task<int> CountAsync(Guid ownerId, CancellationToken token = default)
        {
            return await _context.OwnerBankAccount.CountAsync(x => x.OwnerId == ownerId, token);
        }

        public Task DeleteAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default)
        {
            _context.OwnerBankAccount.Remove(ownerBankAccount);
            return Task.CompletedTask;
        }

        public async Task<List<OwnerBankAccount?>> GetAllByOwnerId(Guid OwnernerId, CancellationToken token = default)
        {
            return await _context.OwnerBankAccount.Where(x => x.OwnerId == OwnernerId).ToListAsync(token);
        }

        public Task<OwnerBankAccount?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _context.OwnerBankAccount.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<OwnerBankAccount?> GetDefaultByOwnerIdAsync(Guid ownerId, CancellationToken token = default)
        {
            return _context.OwnerBankAccount.Where(x => x.OwnerId == ownerId && x.IsDefault).FirstOrDefaultAsync(token);
        }

        public Task UpdateAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default)
        {
            _context.OwnerBankAccount.Update(ownerBankAccount);
            return Task.CompletedTask;
        }
    }
}
