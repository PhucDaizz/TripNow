using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Domain.Repositories
{
    public interface IEscrowAccountRepository
    {
        Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(EscrowAccount escrowAccount, CancellationToken token = default);
        Task<EscrowAccount?> GetByBookingIdAsync(Guid bookingId, CancellationToken token = default);
        Task UpdateAsync(EscrowAccount escrowAccount, CancellationToken token = default);
        Task DeleteAsync(EscrowAccount escrowAccount, CancellationToken token = default);
        Task<PagedResult<EscrowAccount>> GetPagedListAsync(
            int pageNumber, int pageSize,
            EscrowStatus? status, Guid? bookingId,
            DateTime? fromDate, DateTime? toDate,
            CancellationToken token);
    }
}
