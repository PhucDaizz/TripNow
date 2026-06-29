using PaymentService.Domain.Entities;

namespace PaymentService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<EscrowAccount> EscrowAccounts { get; }
        IQueryable<OwnerWallet> OwnerWallets { get; }
        IQueryable<PaymentTransaction> PaymentTransactions { get; }
        IQueryable<Payout> Payouts { get; }
        IQueryable<SettlementItem> SettlementItems { get; }
        IQueryable<SettlementPeriod> SettlementPeriods { get; }
        IQueryable<WalletLedger> WalletLedgers { get; }
        IQueryable<OwnerBankAccount> OwnerBankAccounts { get; }
        IQueryable<RefundRequest> RefundRequests { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
