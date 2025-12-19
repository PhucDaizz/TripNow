namespace Application.DTOs.StaffProfile
{
    public class UpdateStaffProfileDto
    {
        public string NewPosition { get; set; } = default!;
        public Guid HotelId { get; set; }
    }
}
