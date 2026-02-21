using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface ISettlementPeriodRepository
    {
        Task<SettlementPeriod?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(SettlementPeriod settlementPeriod, CancellationToken token = default);
        Task UpdateAsync(SettlementPeriod settlementPeriod, CancellationToken token = default);
        Task DeleteAsync(SettlementPeriod settlementPeriod, CancellationToken token = default);
        Task<PagedResult<SettlementPeriod>> GetPagedListAsync(
            Guid? ownerId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, CancellationToken token);
        Task<SettlementPeriod?> GetByIdWithItemsAsync(Guid id, CancellationToken token = default);
    }
}
