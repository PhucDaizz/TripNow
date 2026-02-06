using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Domain.Repositories
{
    public interface IPayoutRepository
    {
        Task<Payout?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Payout payout, CancellationToken token = default);
        Task<Payout?> GetBySettlementAsync(Guid ownerId, CancellationToken token = default);
        Task UpdateAsync(Payout payout, CancellationToken token = default);
        Task DeleteAsync(Payout payout, CancellationToken token = default);
        Task<PagedResult<Payout>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            PayoutStatus? status,
            Guid? ownerWalletId,
            string? searchTransactionRef,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken token);
    }
}
