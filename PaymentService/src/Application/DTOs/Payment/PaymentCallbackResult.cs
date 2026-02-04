namespace PaymentService.Application.DTOs.Payment
{
    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; init; }
        public string? TransactionId { get; init; }
        public string? FailureReason { get; init; }
    }

}
