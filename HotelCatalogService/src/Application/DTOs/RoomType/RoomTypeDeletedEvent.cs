namespace HotelCatalogService.Application.DTOs.RoomType
{
    public class RoomTypeDeletedEvent
    {
        public Guid RoomTypeId { get; init; }
        public Guid HotelId { get; init; }
    }
}
