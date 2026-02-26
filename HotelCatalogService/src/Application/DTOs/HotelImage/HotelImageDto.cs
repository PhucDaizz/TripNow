namespace HotelCatalogService.Application.DTOs.HotelImage
{
    public class HotelImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsThumbnail { get; set; }
        public string? Caption { get; set; }
    }
}
