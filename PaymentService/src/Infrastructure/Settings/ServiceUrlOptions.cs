namespace PaymentService.Infrastructure.Settings
{
    public class ServiceUrlOptions
    {
        public const string SectionName = "ServiceUrls";
        public string HotelCatalog { get; set; } = default!;
    }
}
