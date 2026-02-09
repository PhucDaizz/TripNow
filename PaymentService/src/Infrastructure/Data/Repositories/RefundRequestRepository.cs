using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class RefundRequestRepository : IRefundRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            await _context.RefundRequest.AddAsync(refundRequest);
        }

        public Task DeleteAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            _context.RefundRequest.Remove(refundRequest);
            return Task.CompletedTask;
        }

        public async Task<RefundRequest?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.RefundRequest.FirstOrDefaultAsync(rr => rr.Id == id, token);
        }

        public async Task<PagedResult<RefundRequest>> GetPagedListAsync(
            int pageNumber, int pageSize,
            RefundStatus? status, string? searchBookingId,
            Guid? userId, string? transactionRef, DateTime? fromDate, DateTime? toDate, 
            CancellationToken token)
        {
            var query = _context.RefundRequest.AsNoTracking();

            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(searchBookingId) && Guid.TryParse(searchBookingId, out var bid))
            {
                query = query.Where(x => x.BookingId == bid);
            }

            if (!string.IsNullOrEmpty(transactionRef))
            {
                query = query.Where(x =>
                    x.OriginalGatewayTransactionRef.Contains(transactionRef) ||
                    (x.RefundGatewayTransactionRef != null && x.RefundGatewayTransactionRef.Contains(transactionRef))
                );
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate.Value); 
            }
            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= toDate.Value);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(token);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return new PagedResult<RefundRequest>(items, totalCount, pageNumber, pageSize);
        }

        public Task UpdateAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            _context.RefundRequest.Update(refundRequest);
            return Task.CompletedTask;
        }
    }
}
