namespace HotelCatalogService.Application.DTOs.Room
{
    public class RollbackCheckInRoomRequest
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
    }
}
