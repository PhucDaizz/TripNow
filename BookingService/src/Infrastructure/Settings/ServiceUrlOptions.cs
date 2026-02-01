namespace BookingService.Infrastructure.Settings
{
    public class ServiceUrlOptions
    {
        public const string SectionName = "ServiceUrls";
        public string HotelCatalog { get; set; } = default!;
        public string Payment { get; set; } = default!;
    }
}
