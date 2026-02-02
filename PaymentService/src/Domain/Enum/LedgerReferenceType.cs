namespace PaymentService.Domain.Enum
{
    public enum LedgerReferenceType : byte
    {
        Booking = 1,      // Cộng tiền từ doanh thu phòng
        Payout = 2,       // Trừ tiền rút về ngân hàng
        Refund = 3,       // Trừ tiền do hoàn trả khách (nếu có trừ ví)
        Adjustment = 4,   // Admin điều chỉnh thủ công (Sửa sai, phạt, thưởng...) 
        Settlement = 5    // Tiền được giải phóng từ ví chờ sang ví chính (nếu muốn track riêng)
    }
}
