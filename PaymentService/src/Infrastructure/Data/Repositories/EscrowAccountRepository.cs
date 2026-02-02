using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class EscrowAccountRepository : IEscrowAccountRepository
    {
        private readonly IApplicationDbContext _context;

        public EscrowAccountRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            await _context.EscrowAccount.AddAsync(escrowAccount, token);
        }

        public Task DeleteAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            _context.EscrowAccount.Remove(escrowAccount);
            return Task.CompletedTask;
        }

        public async Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.EscrowAccount.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            _context.EscrowAccount.Update(escrowAccount);
            return Task.CompletedTask;
        }
    }
}
