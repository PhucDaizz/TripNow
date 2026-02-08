
using PaymentService.Domain.Enum;

namespace PaymentService.Application.DTOs.RefundRequest
{
    public class RefundRequestDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string OriginalGatewayTransactionRef { get; set; } // Mã VNPAY gốc
        public string? RefundGatewayTransactionRef { get; set; }  // Mã hoàn tiền (nếu đã xong)
        public RefundStatus Status { get; set; }
        public string Reason { get; set; }     // Lý do khách/hệ thống yêu cầu
        public string? AdminNote { get; set; } // Ghi chú của Admin
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
