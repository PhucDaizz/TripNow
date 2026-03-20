namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class UserViewedHotelIntegrationEvent
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
