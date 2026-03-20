namespace RecommendationService.Application.DTOs.UserViewedHotel
{
    public class UserViewedHotelDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
