using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Domain.Repositories
{
    public interface IOwnerWalletRepository
    {
        Task<OwnerWallet?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<OwnerWallet?> GetByOwnerIdAsync(Guid id, CancellationToken token = default);
        Task<List<Guid>> GetAllActiveOwnerIdsAsync(CancellationToken token);
        Task<List<WalletLedger>> GetPendingLedgersForSettlementAsync(
            Guid ownerId,
            DateTime cutOffDate,
            CancellationToken token);
        Task AddAsync(OwnerWallet ownerWallet, CancellationToken token = default);
        Task UpdateAsync(OwnerWallet ownerWallet, CancellationToken token = default);
        Task DeleteAsync(OwnerWallet ownerWallet, CancellationToken token = default);

        Task<PagedResult<WalletLedger>> GetPagedListAsync(
            Guid ownerId, int pageNumber, int pageSize,
            DateTime? fromDate, DateTime? toDate, string? type,
            CancellationToken token);

        Task<bool> HasTransactionAsync(
            Guid ownerId,
            Guid referenceId,
            LedgerReferenceType type,
            CancellationToken cancellationToken = default);

        Task<OwnerWallet?> GetWalletWithPendingLedgersAsync(
            Guid ownerId,
            DateTime cutOffDate,
            CancellationToken token);
    }
}
