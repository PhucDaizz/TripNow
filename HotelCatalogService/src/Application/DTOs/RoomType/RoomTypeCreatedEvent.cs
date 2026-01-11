namespace HotelCatalogService.Application.DTOs.RoomType
{
    public class RoomTypeCreatedEvent
    {
        public Guid RoomTypeId { get; init; }
        public Guid HotelId { get; init; }
        public int InitialStock { get; init; }
    }
}
