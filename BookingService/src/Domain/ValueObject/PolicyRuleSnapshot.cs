namespace BookingService.Domain.ValueObject
{
    public class PolicyRuleSnapshot
    {
        public int HoursBeforeCheckIn { get; set; }
        public decimal RefundPercentage { get; set; }
    }
}
