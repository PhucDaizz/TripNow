using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IOwnerWalletRepository
    {
        Task<OwnerWallet?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(OwnerWallet ownerWallet, CancellationToken token = default);
        Task UpdateAsync(OwnerWallet ownerWallet, CancellationToken token = default);
        Task DeleteAsync(OwnerWallet ownerWallet, CancellationToken token = default);
    }
}
