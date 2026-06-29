using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Common;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDomainEventService _domainEventService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<EscrowAccount> EscrowAccount { get; set; } 
        public DbSet<OwnerWallet> OwnerWallet { get; set; } 
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; } 
        public DbSet<Payout> Payout { get; set; } 
        public DbSet<SettlementItem> SettlementItem { get; set; } 
        public DbSet<SettlementPeriod> SettlementPeriod { get; set; } 
        public DbSet<WalletLedger> WalletLedger { get; set; } 
        public DbSet<OwnerBankAccount> OwnerBankAccount { get; set; }
        public DbSet<RefundRequest> RefundRequest { get; set; }

        public IQueryable<EscrowAccount> EscrowAccounts => EscrowAccount;
        public IQueryable<OwnerWallet> OwnerWallets => OwnerWallet;
        public IQueryable<PaymentTransaction> PaymentTransactions => PaymentTransaction;
        public IQueryable<Payout> Payouts => Payout;
        public IQueryable<SettlementItem> SettlementItems => SettlementItem;
        public IQueryable<SettlementPeriod> SettlementPeriods => SettlementPeriod;
        public IQueryable<WalletLedger> WalletLedgers => WalletLedger;
        public IQueryable<OwnerBankAccount> OwnerBankAccounts => OwnerBankAccount;
        public IQueryable<RefundRequest> RefundRequests => RefundRequest;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // write your customizations here    
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _domainEventService.PublishAsync(domainEvent, cancellationToken);
            }
        }
    }
}
