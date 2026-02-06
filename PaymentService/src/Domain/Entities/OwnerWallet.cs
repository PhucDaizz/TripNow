using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class OwnerWallet: BaseEntity, AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public decimal AvailableBalance { get; private set; }
        public decimal PendingBalance { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;

        private readonly List<WalletLedger> _walletLedgers = new();
        public IReadOnlyCollection<WalletLedger> WalletLedgers => _walletLedgers.AsReadOnly();

        private OwnerWallet() { }

        public OwnerWallet(Guid ownerId)
        {
            OwnerId = ownerId;
            AvailableBalance = 0;
            PendingBalance = 0;
        }

        public void ReceiveRevenue(Guid bookingId, decimal amount, decimal transactionGrossAmount, decimal transactionFee , string? description)
        {
            if (amount <= 0) throw new DomainException("Doanh thu nhận được phải lớn hơn 0.");

            PendingBalance += amount;

            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                LedgerDirection.Credit, 
                amount,
                transactionGrossAmount,
                transactionFee,
                LedgerReferenceType.Booking,
                bookingId,
                currentTotal,
                description
            );

            _walletLedgers.Add(ledger);
        }

        public void ReleaseSettlement(Guid settlementId, decimal amount, decimal transactionGrossAmount, decimal transactionFee)
        {
            if (amount <= 0) throw new DomainException("Số tiền đối soát phải > 0.");
            if (PendingBalance < amount) throw new DomainException("Số dư chờ không đủ để đối soát (Lỗi nghiêm trọng).");

            PendingBalance -= amount;   
            AvailableBalance += amount; 

            var currentTotal = AvailableBalance + PendingBalance;
            var ledger = new WalletLedger(
               this.Id,
               LedgerDirection.Credit,
               amount,
               transactionGrossAmount,
               transactionFee,
               LedgerReferenceType.Settlement, // Loại tham chiếu: Đối soát
               settlementId,
               currentTotal
           );
            _walletLedgers.Add(ledger);
        }

        public void DebitForPayout(Guid payoutId, decimal amount, decimal transactionGrossAmount, decimal transactionFee)
        {
            if (amount <= 0) throw new DomainException("Số tiền rút phải > 0.");
            if (AvailableBalance < amount) throw new DomainException("Số dư khả dụng không đủ.");

            AvailableBalance -= amount;

            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                LedgerDirection.Debit, 
                amount,
                transactionGrossAmount,
                transactionFee,
                LedgerReferenceType.Payout,
                payoutId,
                currentTotal
            );

            _walletLedgers.Add(ledger);
        }

        public void AdjustBalance(decimal amount, decimal transactionGrossAmount, decimal transactionFee, string reasonRef)
        {
            AvailableBalance += amount;

            var direction = amount >= 0 ? LedgerDirection.Credit : LedgerDirection.Debit;
            var absAmount = Math.Abs(amount);
            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                direction,
                absAmount,
                transactionGrossAmount,
                transactionFee,
                LedgerReferenceType.Adjustment,
                Guid.NewGuid(), 
                currentTotal,
                reasonRef
            );
            _walletLedgers.Add(ledger);
        }
    }
}
