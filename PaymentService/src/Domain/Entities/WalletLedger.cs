using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class WalletLedger: BaseEntity
    {
        public Guid WalletId { get; private set; }
        public LedgerDirection Direction { get; private set; } // Chiều giao dịch: Credit (Cộng tiền/Báo có) hoặc Debit (Trừ tiền/Báo nợ)
        public decimal Amount { get; private set; }
        public LedgerReferenceType ReferenceType { get; private set; } // Loại nghiệp vụ: BookingRevenue, Payout, Adjustment, Refund...
        public Guid ReferenceId { get; private set; }
        public decimal BalanceAfter { get; private set; }
        public string? Description { get; private set; }
        public Guid? SettlementPeriodId { get; private set; } // Liên kết đến kỳ đối soát nếu có
        public decimal TransactionGrossAmount { get; private set; } 
        public decimal TransactionFee { get; private set; }

        private WalletLedger() { }

        internal WalletLedger(Guid walletId, LedgerDirection direction, decimal amount, decimal transactionGrossAmount, decimal transactionFee,
                              LedgerReferenceType referenceType, Guid referenceId, decimal balanceAfter, string? description = null)
        {
            if (amount <= 0) throw new DomainException("Số tiền ghi sổ cái phải lớn hơn 0.");
            if (walletId == Guid.Empty) throw new DomainException("WalletId không hợp lệ.");
            if (referenceId == Guid.Empty) throw new DomainException("ReferenceId không hợp lệ.");

            WalletId = walletId;
            Direction = direction;
            Amount = amount;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            BalanceAfter = balanceAfter;
            Description = description;
            TransactionGrossAmount = transactionGrossAmount;
            TransactionFee = transactionFee;
        }

        public void MarkAsSettled(Guid settlementPeriodId)
        {
            if (SettlementPeriodId != null)
                throw new DomainException("Giao dịch này đã được đối soát rồi.");

            SettlementPeriodId = settlementPeriodId;
        }
    }
}
