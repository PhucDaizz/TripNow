namespace BookingService.Domain.Enum
{
    public enum PaymentStatus
    {
        Unpaid = 0, // Chưa thanh toán
        Paid = 1,       // Tiền đã vào Escrow
        RefundPending = 2, // Chờ hoàn tiền
        Refunded = 3,    // Đã hoàn tiền
        Cancelled = 4,     // Hủy giao dịch (khi chưa trả tiền)
        PartialRefunded = 5 // Hoàn tiền một phần
    }
}
