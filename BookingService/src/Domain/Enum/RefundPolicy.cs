namespace BookingService.Domain.Enum
{
    public enum RefundPolicy
    {
        Free = 0,           // Hoàn 100%
        NonRefundable = 1,  // Không hoàn
        Partial = 2         // Hoàn 1 phần
    }
}
