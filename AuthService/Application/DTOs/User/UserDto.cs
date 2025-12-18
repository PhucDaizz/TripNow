namespace Application.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public bool? Gender { get; set; }

        // Thông tin nhân viên (nếu có)
        public Guid? HotelId { get; set; }
        public string? Position { get; set; }

        // Role (Admin, HotelOwner, Staff, User...)
        public string Role { get; set; }
    }
}
