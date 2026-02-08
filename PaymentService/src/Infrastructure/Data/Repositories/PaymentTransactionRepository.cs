using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
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
        public async Task<PaymentTransaction?> GetByBookingIdAsync(Guid bookingId, CancellationToken token = default)
        {
            return await _context.PaymentTransaction.FirstOrDefaultAsync(x => x.BookingId == bookingId, token);
        }

        public async Task<PaymentTransaction?> GetByBookingWithStatusAsync(Guid bookingId, PaymentTransactionStatus status, CancellationToken token = default)
        {
            return await _context.PaymentTransaction.FirstOrDefaultAsync(x => x.BookingId == bookingId &&
                    x.TransactionStatus == PaymentTransactionStatus.Success, token);
        }

        public async Task<PaymentTransaction?> GetByIdWithStatusAsync(Guid id, PaymentTransactionStatus status, CancellationToken token = default)
        {
            return await _context.PaymentTransaction.FirstOrDefaultAsync(x => x.Id == id &&
                    x.TransactionStatus == PaymentTransactionStatus.Pending, token);
        }

        public Task<PaymentTransaction?> GetByMerchantRefAsync(string merchantRef, CancellationToken token = default)
        {
            return _context.PaymentTransaction.FirstOrDefaultAsync(x => x.MerchantRef == merchantRef, token);
        }

        public Task UpdateAsync(PaymentTransaction paymentTransaction, CancellationToken token = default)
        {
            _context.PaymentTransaction.Update(paymentTransaction);
            return Task.CompletedTask;
        }

        public Task<PaymentTransaction?> GetSuccessTransactionByBookingIdAsync(Guid bookingId, CancellationToken token = default)
        {
            return _context.PaymentTransaction.FirstOrDefaultAsync(x => x.BookingId == bookingId &&
                    x.TransactionStatus == PaymentTransactionStatus.Success, token);
        }
    }
}
