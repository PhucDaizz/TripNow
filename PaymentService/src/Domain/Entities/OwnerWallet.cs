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

        public void ReceiveRevenue(Guid bookingId, decimal amount, string? description)
        {
            if (amount <= 0) throw new DomainException("Doanh thu nhận được phải lớn hơn 0.");

            PendingBalance += amount;

            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                LedgerDirection.Credit, 
                amount,
                LedgerReferenceType.Booking,
                bookingId,
                currentTotal,
                description
            );

            _walletLedgers.Add(ledger);
        }

        public void ReleaseSettlement(Guid settlementId, decimal amount)
        {
            if (amount <= 0) throw new DomainException("Số tiền đối soát phải > 0.");
            if (PendingBalance < amount) throw new DomainException("Số dư chờ không đủ để đối soát (Lỗi nghiêm trọng).");

            PendingBalance -= amount;   
            AvailableBalance += amount; 

            // Ghi sổ: Loại này đặc biệt, là chuyển đổi trạng thái tiền, nhưng tổng tài sản không đổi
            // Tuy nhiên, thường ta sẽ ghi nhận là Credit vào Available để dễ theo dõi dòng tiền khả dụng.
            // Hoặc có thể không ghi Ledger nếu bạn chỉ quan tâm tổng tài sản. 
            // Nhưng tốt nhất nên ghi log loại "SettlementRelease".

            var currentTotal = AvailableBalance + PendingBalance;
            var ledger = new WalletLedger(
               this.Id,
               LedgerDirection.Credit,
               amount,
               LedgerReferenceType.Settlement, // Loại tham chiếu: Đối soát
               settlementId,
               currentTotal
           );
            _walletLedgers.Add(ledger);
        }

        public void DebitForPayout(Guid payoutId, decimal amount)
        {
            if (amount <= 0) throw new DomainException("Số tiền rút phải > 0.");
            if (AvailableBalance < amount) throw new DomainException("Số dư khả dụng không đủ.");

            AvailableBalance -= amount;

            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                LedgerDirection.Debit, 
                amount,
                LedgerReferenceType.Payout,
                payoutId,
                currentTotal
            );

            _walletLedgers.Add(ledger);
        }

        public void AdjustBalance(decimal amount, string reasonRef)
        {
            AvailableBalance += amount;

            var direction = amount >= 0 ? LedgerDirection.Credit : LedgerDirection.Debit;
            var absAmount = Math.Abs(amount);
            var currentTotal = AvailableBalance + PendingBalance;

            var ledger = new WalletLedger(
                this.Id,
                direction,
                absAmount,
                LedgerReferenceType.Adjustment,
                Guid.NewGuid(), 
                currentTotal,
                reasonRef
            );
            _walletLedgers.Add(ledger);
        }
    }
}
