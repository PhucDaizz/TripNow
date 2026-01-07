namespace BookingService.Domain.Enum
{
    public enum BookingStatus
    {
        Pending = 0,    // Vừa tạo, chưa thanh toán
        Confirmed = 1,  // Đã thanh toán/giữ chỗ
        Cancelled = 2,  // Đã hủy
        Completed = 3,  // Đã check-out xong
        NoShow = 4      // Khách không đến
    }
}
