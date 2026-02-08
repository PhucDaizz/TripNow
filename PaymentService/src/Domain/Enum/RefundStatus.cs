namespace PaymentService.Domain.Enum
{
    public enum RefundStatus: byte
    {
        Pending = 0,
        Completed = 1,
        Rejected = 2
    }
}
