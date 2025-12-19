namespace Application.DTOs.StaffProfile
{
    public class StaffProfileDto
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Guid HotelId { get; init; }
        public string Position { get; init; }
    }
}
