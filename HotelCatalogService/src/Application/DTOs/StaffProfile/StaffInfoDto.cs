namespace HotelCatalogService.Application.DTOs.StaffProfile
{
    public class StaffInfoDto
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Guid HotelId { get; init; }
        public string Position { get; init; }
    }
}
