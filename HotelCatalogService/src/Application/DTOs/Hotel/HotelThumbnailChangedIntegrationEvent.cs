namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class HotelThumbnailChangedIntegrationEvent
    {
        public Guid  HotelId { get; set; }
        public string ImageUrl { get; set; }
    }
}
