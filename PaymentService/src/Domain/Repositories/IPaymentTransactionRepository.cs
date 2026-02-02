using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task AddAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
        Task UpdateAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
        Task DeleteAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
    }
}
