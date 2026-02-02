namespace PaymentService.Domain.Enum
{
    public enum EscrowStatus: byte
    {
        Holding = 0,
        Released = 1,
        Refunded = 2
    }
}
