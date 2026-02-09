namespace PaymentService.Application.DTOs.EscrowAccount
{
    public class EscrowAccountDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }          // Tổng tiền khách trả
        public decimal RefundedAmount { get; set; }  // Số tiền đã hoàn lại khách
        public decimal ProviderFee { get; set; }     // Phí sàn hiện tại
        public string Status { get; set; }           // Holding, Released, Refunded...

        public decimal ActualRevenue { get; set; }   // Doanh thu thực (Amount - Refunded)
        public decimal NetToOwner { get; set; }      // Tiền sẽ về ví Owner (Actual - Fee)

        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
