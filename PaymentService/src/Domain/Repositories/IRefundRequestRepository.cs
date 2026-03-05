using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Domain.Repositories
{
    public interface IRefundRequestRepository
    {
        Task<RefundRequest?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(RefundRequest refundRequest, CancellationToken token = default);
        Task UpdateAsync(RefundRequest refundRequest, CancellationToken token = default);
        Task DeleteAsync(RefundRequest refundRequest, CancellationToken token = default);
        Task<PagedResult<RefundRequest>> GetPagedListAsync(
            int pageNumber, int pageSize,
            RefundStatus? status, string? searchBookingId,
            Guid? userId, string? transactionRef, DateTime? fromDate, DateTime? toDate, 
            CancellationToken token);

        Task<bool> ExistsByBookingIdAsync(Guid bookingId, CancellationToken token = default);
    }
}
