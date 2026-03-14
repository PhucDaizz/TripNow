using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Application.DTOs.Room
{
    public class RoomStatusUpdateDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = null!; 
        public Guid BlockId { get; set; }
        public string BlockName { get; set; }
        public Guid FloorId { get; set; }
        public string FloorName { get; set; }

        public string Status { get; set; } 
        public Guid? AssignedToStaffId { get; set; } 
        public string? AssignedToStaffName { get; set; } 
    }
}
