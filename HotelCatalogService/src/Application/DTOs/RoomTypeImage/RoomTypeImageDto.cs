namespace HotelCatalogService.Application.DTOs.RoomTypeImage
{
    public class RoomTypeImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMainImage { get; set; }
    }
}
