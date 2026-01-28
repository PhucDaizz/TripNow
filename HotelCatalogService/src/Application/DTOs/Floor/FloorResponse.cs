using HotelCatalogService.Application.DTOs.Room;

namespace HotelCatalogService.Application.DTOs.Floor
{
    public class FloorResponse
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; } // 1, 2, 3
        public List<RoomResponse> Rooms { get; set; } = new();
    }
}
