using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly IApplicationDbContext _context;

        public PaymentTransactionRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentTransaction paymentTransaction, CancellationToken token = default)
        {
            await _context.PaymentTransaction.AddAsync(paymentTransaction, token);
        }

        public Task DeleteAsync(PaymentTransaction paymentTransaction, CancellationToken token = default)
        {
            _context.PaymentTransaction.Remove(paymentTransaction);
            return Task.CompletedTask;
        }

        public async Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.PaymentTransaction.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(PaymentTransaction paymentTransaction, CancellationToken token = default)
        {
            _context.PaymentTransaction.Update(paymentTransaction);
            return Task.CompletedTask;
        }
    }
}
