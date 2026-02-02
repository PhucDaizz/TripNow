namespace PaymentService.Domain.Enum
{
    public enum PayoutStatus : byte
    {
        Pending = 0,      // Tạo yêu cầu
        Processing = 1,   // Đang xử lý chi trả
        Completed = 2,    // Đã chi trả thành công
        Failed = 3,       // Thất bại
        Cancelled = 4     // Bị huỷ
    }
}
