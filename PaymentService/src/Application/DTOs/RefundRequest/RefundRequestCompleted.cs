namespace PaymentService.Application.DTOs.RefundRequest
{
    public class RefundRequestCompleted
    {
        public Guid RefundId { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserRefundId { get; set; }
        public decimal AmountRefund { get; set; }
    }
}
