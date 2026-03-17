namespace PaymentService.Domain.Enum
{
    public enum NotificationType : byte
    {
        System = 1,
        Booking = 2,
        Payment = 3,
        Promotion = 4,
        LocationVerified = 5
    }
}
