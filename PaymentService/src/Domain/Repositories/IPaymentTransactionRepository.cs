using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using System.Threading.Tasks;

namespace PaymentService.Domain.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<PaymentTransaction?> GetByBookingIdAsync(Guid bookingId, CancellationToken token = default);
        Task<PaymentTransaction?> GetByBookingWithStatusAsync(Guid bookingId, PaymentTransactionStatus status, CancellationToken token = default);
        Task<PaymentTransaction?> GetByMerchantRefAsync(string merchantRef, CancellationToken token = default);
        Task<PaymentTransaction?> GetByIdWithStatusAsync(Guid id, PaymentTransactionStatus status, CancellationToken token = default);
        Task AddAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
        Task UpdateAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
        Task DeleteAsync(PaymentTransaction paymentTransaction, CancellationToken token = default);
    }
}
