namespace PaymentService.Domain.Enum
{
    public enum PaymentTransactionStatus: byte
    {
        Pending = 0,
        Success = 1,
        Failed = 2,
        Refunded = 3,
        Cancelled = 4
    }
}
