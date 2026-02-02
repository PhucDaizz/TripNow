using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class PaymentTransaction: BaseEntity, AggregateRoot
    {
        public Guid? PayerUserId { get; private set; }
        public Guid BookingId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public string? ProviderTxnId { get; private set; }
        public decimal? ProviderFee { get; private set; }
        public string? RawResponse { get; private set; }
        public PaymentTransactionStatus TransactionStatus { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime? PaymentDate { get; private set; }

        private PaymentTransaction() { }

        internal PaymentTransaction(Guid bookingId, decimal amount, PaymentProvider provider, Guid? payerUserId = null)
        {
            if (amount <= 0) throw new DomainException("Số tiền giao dịch phải lớn hơn 0.");

            BookingId = bookingId;
            Amount = amount;
            Provider = provider;
            PayerUserId = payerUserId;
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

            // bắn Domain Event để bên Escrow biết mà giữ tiền
            // AddDomainEvent(new PaymentSucceededEvent(this.Id, this.BookingId, this.Amount));
        }

        public void MarkAsFailed(string rawResponse, string reason)
        {
            if (TransactionStatus == PaymentTransactionStatus.Success)
                throw new DomainException("Không thể đánh dấu thất bại cho giao dịch đã thành công.");

            RawResponse = rawResponse;
            FailureReason = reason;
            TransactionStatus = PaymentTransactionStatus.Failed;
        }

        public void Refund()
        {
            if (TransactionStatus != PaymentTransactionStatus.Success)
                throw new DomainException("Chỉ có thể hoàn tiền cho giao dịch đã thành công.");

            TransactionStatus = PaymentTransactionStatus.Refunded;

            // AddDomainEvent(new PaymentRefundedEvent(this.Id, this.BookingId));
        }
    }
}
