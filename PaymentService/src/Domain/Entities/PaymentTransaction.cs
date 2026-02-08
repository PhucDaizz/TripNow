using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.PaymentTransaction;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class PaymentTransaction: BaseEntity, AggregateRoot
    {
        public Guid? PayerUserId { get; private set; }
        public Guid BookingId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public TransactionType Type { get; private set; }
        public string? MerchantRef { get; private set; }
        public string? ProviderTxnId { get; private set; }
        public decimal? ProviderFee { get; private set; }
        public string? RawResponse { get; private set; }
        public PaymentTransactionStatus TransactionStatus { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string? Note { get; private set; }

        private PaymentTransaction() { }

        public PaymentTransaction(Guid bookingId, decimal amount, PaymentProvider provider, string? merchantRef, Guid? payerUserId = null)
        {
            if (amount <= 0) throw new DomainException("Số tiền giao dịch phải lớn hơn 0.");

            BookingId = bookingId;
            Amount = amount;
            Provider = provider;
            MerchantRef = merchantRef;
            PayerUserId = payerUserId;
            Type = TransactionType.Payment;
            TransactionStatus = PaymentTransactionStatus.Pending;
        }

        public PaymentTransaction(Guid bookingId, decimal amount, PaymentProvider provider, TransactionType type, string? refCode, string? note)
        {
            BookingId = bookingId;
            Amount = amount;
            Provider = provider;
            Type = type;
            MerchantRef = refCode;
            Note = note;
            TransactionStatus = PaymentTransactionStatus.Pending;
        }

        public void ProcessCallback(string providerTxnId, string rawResponse, decimal? providerFee = 0)
        {
            if (TransactionStatus == PaymentTransactionStatus.Success)
            {
                if (ProviderTxnId != providerTxnId)
                    throw new DomainException("Xung đột dữ liệu: TransactionId không khớp với lần xử lý trước.");
                return;
            }

            ProviderTxnId = providerTxnId;
            RawResponse = rawResponse;
            ProviderFee = providerFee;
            TransactionStatus = PaymentTransactionStatus.Success;
            PaymentDate = DateTime.UtcNow;

            AddDomainEvent(new PaymentSucceededEvent(this.BookingId, this.Amount, providerFee ?? 0m));
        }

        public void MarkAsFailed(string rawResponse, string reason)
        {
            if (TransactionStatus == PaymentTransactionStatus.Success)
                throw new DomainException("Không thể đánh dấu thất bại cho giao dịch đã thành công.");

            RawResponse = rawResponse;
            FailureReason = reason;
            TransactionStatus = PaymentTransactionStatus.Failed;
        }

        public void SetMerchantRef(string merchantRef)
        {
            MerchantRef = merchantRef;
        }

        public void UpdatePaymentDate()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (TransactionStatus != PaymentTransactionStatus.Pending)
                throw new DomainException("Chỉ có thể hủy giao dịch đang chờ xử lý.");
            TransactionStatus = PaymentTransactionStatus.Cancelled;
            FailureReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Refund()
        {
            if (TransactionStatus != PaymentTransactionStatus.Success)
                throw new DomainException("Chỉ có thể hoàn tiền cho giao dịch đã thành công.");

            TransactionStatus = PaymentTransactionStatus.Refunded;

            // AddDomainEvent(new PaymentRefundedEvent(this.Id, this.BookingId));
        }

        public void MarkAsManualSuccess(string providerTxnId)
        {
            if (TransactionStatus == PaymentTransactionStatus.Success) return;

            ProviderTxnId = providerTxnId; 
            TransactionStatus = PaymentTransactionStatus.Success;
            PaymentDate = DateTime.UtcNow;
        }
    }
}
