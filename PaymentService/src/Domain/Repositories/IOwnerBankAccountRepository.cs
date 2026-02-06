using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IOwnerBankAccountRepository
    {
        Task<OwnerBankAccount?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default);
        Task<OwnerBankAccount?> GetDefaultByOwnerIdAsync(Guid ownerId, CancellationToken token = default);
        Task UpdateAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default);
        Task DeleteAsync(OwnerBankAccount ownerBankAccount, CancellationToken token = default);
    }
}
