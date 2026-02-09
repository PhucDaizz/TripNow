using Microsoft.EntityFrameworkCore;
using Nexus.BuildingBlocks.Model;
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

        public async Task<Domain.Common.Models.PagedResult<PaymentTransaction>> GetPagedListAsync(int pageNumber, int pageSize, Guid? userId, Guid? bookingId, PaymentTransactionStatus? status, TransactionType? type, DateTime? fromDate, DateTime? toDate, CancellationToken token)
        {
            var query = _context.PaymentTransaction.AsNoTracking();

            if (userId.HasValue) query = query.Where(x => x.PayerUserId == userId);
            if (bookingId.HasValue) query = query.Where(x => x.BookingId == bookingId);
            if (status.HasValue) query = query.Where(x => x.TransactionStatus == status);
            if (type.HasValue) query = query.Where(x => x.Type == type);

            if (fromDate.HasValue) query = query.Where(x => x.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.CreatedAt <= toDate.Value);

            query = query.OrderByDescending(x => x.CreatedAt);

            var total = await query.CountAsync(token);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return new Domain.Common.Models.PagedResult<PaymentTransaction>(items, total, pageNumber, pageSize);
        }
    }
}
