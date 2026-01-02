namespace HotelCatalogService.Application.DTOs.Room
{
    public record AddRoomInventoryEvent
    {
        public Guid RoomtypeId { get; init; }
    }
}
