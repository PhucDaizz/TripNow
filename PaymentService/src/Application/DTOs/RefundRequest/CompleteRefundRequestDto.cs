namespace PaymentService.Application.DTOs.RefundRequest
{
    public class CompleteRefundRequestDto
    {
        public string RefundGatewayTransactionRef { get; set; }
        public string? Note { get; set; }
    }
}
