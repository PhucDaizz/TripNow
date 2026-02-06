using PaymentService.Domain.Repositories;

namespace PaymentService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISettlementPeriodRepository SettlementPeriods { get; }
        IPaymentTransactionRepository PaymentTransactions {  get; }
        IOwnerWalletRepository OwnerWallets {  get; }
        IEscrowAccountRepository EscrowAccounts { get; }
        IOwnerBankAccountRepository OwnerBankAccounts { get; }
        IPayoutRepository Payouts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
