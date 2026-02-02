using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IEscrowAccountRepository
    {
        Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(EscrowAccount escrowAccount, CancellationToken token = default);
        Task UpdateAsync(EscrowAccount escrowAccount, CancellationToken token = default);
        Task DeleteAsync(EscrowAccount escrowAccount, CancellationToken token = default);
    }
}
