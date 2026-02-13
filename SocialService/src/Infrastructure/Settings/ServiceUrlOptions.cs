namespace SocialService.Infrastructure.Settings
{
    public class ServiceUrlOptions
    {
        public const string SectionName = "ServiceUrls";
        public string HotelCatalog { get; set; } = default!;
        public string Auth { get; set; } = default!;
    }
}
