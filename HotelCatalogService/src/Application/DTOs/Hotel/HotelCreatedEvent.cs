namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class HotelCreatedEvent
    {
        public Guid HotelId { get; init; }
        public required string Name { get; init; }
    }
}
