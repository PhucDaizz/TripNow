namespace BookingService.Application.DTOs.HotelCatalog
{
    public class RoomResponse
    {
        public Guid RoomId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomName { get; set; }
        public string Status { get; set; }
    }
}
