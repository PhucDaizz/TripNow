namespace HotelCatalogService.Application.DTOs.Room
{
    public class CheckInHotelRoomRequest
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public Guid CheckedInBy { get; set; }
    }
}