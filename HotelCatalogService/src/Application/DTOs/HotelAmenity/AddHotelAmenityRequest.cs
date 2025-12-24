namespace HotelCatalogService.Application.DTOs.HotelAmenity
{
    public class AddHotelAmenityRequest
    {
        public Guid AmenityId { get; set; }
        public string? Description { get; set; }
        public bool IsFree { get; set; } = true; 
    }
}
