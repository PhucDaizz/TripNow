namespace PaymentService.Application.DTOs.Payment
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public Guid? PayerUserId { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; } // VNPAY, MOMO
        public string Type { get; set; }     // Payment, Refund
        public string TransactionStatus { get; set; } // Success, Failed
        public string? MerchantRef { get; set; }
        public string? ProviderTxnId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? FailureReason { get; set; }
        public string? Note { get; set; }
    }
}
