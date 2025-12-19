namespace Application.DTOs.StaffProfile
{
    public class CreateStaffProfileDto
    {
        public string Email { get; set; }
        public Guid HotelId { get; set; }
        public string Position { get; set; }
    }
}
