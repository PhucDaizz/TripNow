namespace PaymentService.Application.DTOs.Payment
{
    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; init; }
        public string MerchantRef { get; set; } = string.Empty;
        public string ProviderTxnId { get; set; } = string.Empty;
        public string FailureReason { get; set; } = string.Empty;
        public string RawResponse { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

}
