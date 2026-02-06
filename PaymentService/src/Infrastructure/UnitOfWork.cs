using Microsoft.EntityFrameworkCore.Storage;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public ISettlementPeriodRepository SettlementPeriods { get; }
        public IPaymentTransactionRepository PaymentTransactions { get; }
        public IOwnerWalletRepository OwnerWallets { get; }
        public IEscrowAccountRepository EscrowAccounts { get; }
        public IOwnerBankAccountRepository OwnerBankAccounts { get; }
        public IPayoutRepository Payouts { get; }


        public UnitOfWork(
            ApplicationDbContext context,
            ISettlementPeriodRepository settlementPeriodRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            IOwnerWalletRepository ownerWalletRepository,
            IEscrowAccountRepository escrowAccountRepository,
            IOwnerBankAccountRepository ownerBankAccounts,
            IPayoutRepository payouts)
        {
            _context = context;
            SettlementPeriods = settlementPeriodRepository;
            PaymentTransactions = paymentTransactionRepository;
            OwnerWallets = ownerWalletRepository;
            EscrowAccounts = escrowAccountRepository;
            OwnerBankAccounts = ownerBankAccounts;
            Payouts = payouts;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
