namespace HotelCatalogService.Application.DTOs.HotelAmenity
{
    public class HotelAmenityDto
    {
        public Guid Id { get; set; }
        public Guid AmenityId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string? Icon { get; set; } 
        public string? Description { get; set; }
        public bool IsFree { get; set; }
    }
}
