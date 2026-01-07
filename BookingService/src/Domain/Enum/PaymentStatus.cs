namespace BookingService.Domain.Enum
{
    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,       // Tiền đã vào Escrow
        Refunded = 2    // Đã hoàn tiền
    }
}
