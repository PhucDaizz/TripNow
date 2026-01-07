namespace BookingService.Domain.Enum
{
    public enum CancelledBy
    {
        User = 0,
        Hotel = 1,
        System = 2 // Tự hủy do hết hạn thanh toán
    }
}
