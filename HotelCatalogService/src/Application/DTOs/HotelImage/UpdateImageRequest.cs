namespace HotelCatalogService.Application.DTOs.HotelImage
{
    public class UpdateImageRequest
    {
        public bool IsThumbnail { get; set; }
        public string? Caption { get; set; }
    }
}
