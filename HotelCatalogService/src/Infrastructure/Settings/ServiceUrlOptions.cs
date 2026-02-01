namespace HotelCatalogService.Infrastructure.Settings
{
    public class ServiceUrlOptions
    {
        public const string SectionName = "ServiceUrls";
        public string Auth { get; set; } = default!;
    }
}
