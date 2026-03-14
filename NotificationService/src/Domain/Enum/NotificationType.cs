namespace NotificationService.Domain.Enum
{
    public enum NotificationType: byte
    {
        System = 1,       // Thông báo hệ thống chung
        Booking = 2,      // Thay đổi trạng thái đặt phòng
        Payment = 3,      // Hoàn tiền, trừ tiền, rút tiền
        Promotion = 4,    // Khuyến mãi mới
        LocationVerified = 5 // Địa điểm đã được xác minh
    }
}
