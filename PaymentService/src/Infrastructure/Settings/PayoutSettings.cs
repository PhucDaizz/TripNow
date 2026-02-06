namespace PaymentService.Infrastructure.Settings
{
    public class PayoutSettings
    {
        public const string SectionName = "PayoutConfig";
        public int HoldDays { get; set; }
        public string CronExpression { get; set; }
    }
}
