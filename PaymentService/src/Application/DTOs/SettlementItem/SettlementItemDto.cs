namespace PaymentService.Application.DTOs.SettlementItem
{
    public class SettlementItemDto
    {
        public Guid BookingId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Type { get; set; } // Booking, RefundDeduction...
    }
}
