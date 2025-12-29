namespace HotelCatalogService.Application.DTOs.Room
{
    public class CreateRoomRequest
    {
        public string Name { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
