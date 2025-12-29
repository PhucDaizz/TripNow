using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Domain.Dto.Room
{
    public class DirtyRoomDto
    {
        public Guid RoomId { get; init; }
        public string RoomName { get; init; }
        public Guid FloorId { get; set; }
        public string FloorName { get; init; }
        public Guid BlockId { get; set; }
        public string BlockName { get; init; }
        public string Status { get; init; }
    }
}
