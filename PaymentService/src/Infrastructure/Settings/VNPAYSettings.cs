namespace PaymentService.Infrastructure.Settings
{
    public class VNPAYSettings
    {
        public const string SectionName = "VNPAY";
        public string TmnCode { get; set; } = default!;
        public string HashSecret { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
        public string Version { get; set; } = "2.1.0";
        public string OrderType { get; set; } = "other";
    }
}
