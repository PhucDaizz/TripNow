using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IPayoutRepository
    {
        Task<Payout?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Payout payout, CancellationToken token = default);
        Task<Payout?> GetBySettlementAsync(Guid ownerId, CancellationToken token = default);
        Task UpdateAsync(Payout payout, CancellationToken token = default);
        Task DeleteAsync(Payout payout, CancellationToken token = default);
    }
}
