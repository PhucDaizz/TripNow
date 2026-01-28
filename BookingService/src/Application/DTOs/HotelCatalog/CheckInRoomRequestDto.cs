namespace BookingService.Application.DTOs.HotelCatalog
{
    public class CheckInRoomRequestDto
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public Guid CheckedInBy { get; set; }
    }
}
