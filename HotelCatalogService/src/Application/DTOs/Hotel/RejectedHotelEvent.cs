namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class RejectedHotelEvent
    {
        public Guid OwnerId { get; init; }
        public string HotelName { get; init; }
        public string Reason { get; init; }
    }
}
