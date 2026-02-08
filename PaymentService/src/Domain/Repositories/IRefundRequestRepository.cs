using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IRefundRequestRepository
    {
        Task<RefundRequest?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(RefundRequest refundRequest, CancellationToken token = default);
        Task UpdateAsync(RefundRequest refundRequest, CancellationToken token = default);
        Task DeleteAsync(RefundRequest refundRequest, CancellationToken token = default);
    }
}
