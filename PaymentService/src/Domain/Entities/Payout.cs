using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.Payout;
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

            AddDomainEvent(new PayoutCompletedEvent
                {
                    PayoutId = this.Id, 
                    OwnerWalletId = this.OwnerWalletId, 
                    Amount = this.Amount, 
                    TransactionReference = transactionRef 
                });
        }

        public void MarkAsFailed(string reason)
        {
            if (Status == PayoutStatus.Completed)
            {
                throw new DomainException("Không thể đánh dấu thất bại cho Payout đã thành công.");
            }

            if (Status != PayoutStatus.Pending && Status != PayoutStatus.Processing)
            {
                throw new DomainException("Chỉ được hủy đơn đang chờ xử lý hoặc chờ duyệt.");
            }

            FailureReason = reason;
            Status = PayoutStatus.Failed;

            AddDomainEvent(
                new PayoutRejectedEvent
                {
                    PayoutId = this.Id,
                    OwnerWalletId = this.OwnerWalletId,
                    Amount = this.Amount,
                    Reason = reason
                });
        }

        public void MarkAsProcessing()
        {
            if (Status != PayoutStatus.Pending) return;
            Status = PayoutStatus.Processing;
        }
    }
}
