using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class Payout: BaseEntity, AggregateRoot
    {
        public Guid? SettlementId { get; private set; }
        public Guid OwnerWalletId { get; private set; }
        public string BankInfo { get; private set; }
        public decimal Amount { get; private set; }
        public string? TransactionReference { get; private set; }
        public string? FailureReason { get; private set; }
        public PayoutStatus Status { get; private set; }

        private Payout() { }

        public Payout(Guid settlementId, Guid ownerWalletId, decimal amount, string bankInfo)
        {
            if (amount <= 0) throw new DomainException("Số tiền rút phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(bankInfo)) throw new DomainException("Thông tin ngân hàng không được để trống.");

            SettlementId = settlementId;
            OwnerWalletId = ownerWalletId;
            Amount = amount;
            BankInfo = bankInfo;

            Status = PayoutStatus.Pending;

            // AddDomainEvent(new PayoutCreatedEvent(this.Id, this.Amount));
        }


        public void MarkAsCompleted(string transactionRef)
        {
            if (Status != PayoutStatus.Pending && Status != PayoutStatus.Processing)
            {
                throw new DomainException("Chỉ có thể hoàn tất Payout đang chờ xử lý.");
            }

            TransactionReference = transactionRef;
            Status = PayoutStatus.Completed;
            FailureReason = null; 
        }

        public void MarkAsFailed(string reason)
        {
            if (Status == PayoutStatus.Completed)
            {
                throw new DomainException("Không thể đánh dấu thất bại cho Payout đã thành công.");
            }

            if(Status != PayoutStatus.Processing)
            {
                throw new DomainException("Chỉ được hủy đơn đang chờ xử lý.");
            }

            FailureReason = reason;
            Status = PayoutStatus.Failed;

            // CPayoutFailedEvent (để trigger hoàn tiền vào ví)
            // AddDomainEvent(new PayoutFailedEvent(this.Id, this.SettlementId));
        }

        public void MarkAsProcessing()
        {
            if (Status != PayoutStatus.Pending) return;
            Status = PayoutStatus.Processing;
        }
    }
}
