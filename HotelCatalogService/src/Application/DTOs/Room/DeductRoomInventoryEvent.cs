namespace HotelCatalogService.Application.DTOs.Room
{
    public record DeductRoomInventoryEvent
    {
        public Guid RoomTypeId { get; init; }
    }
}
