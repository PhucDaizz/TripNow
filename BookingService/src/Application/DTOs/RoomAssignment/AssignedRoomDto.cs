namespace BookingService.Application.DTOs.RoomAssignment
{
    public class AssignedRoomDto
    {
        public Guid BookingItemId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
