using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<EscrowAccount> EscrowAccount { get; }
        public DbSet<OwnerWallet> OwnerWallet { get; }
        public DbSet<PaymentTransaction> PaymentTransaction { get; }
        public DbSet<Payout> Payout { get; }
        public DbSet<SettlementItem> SettlementItem { get; }
        public DbSet<SettlementPeriod> SettlementPeriod { get; }
        public DbSet<WalletLedger> WalletLedger { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
