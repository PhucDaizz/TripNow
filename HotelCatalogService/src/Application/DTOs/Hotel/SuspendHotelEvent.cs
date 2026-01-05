namespace HotelCatalogService.Application.DTOs.Hotel
{
    public record SuspendHotelEvent
    {
        public Guid OwnerId { get; init; }
        public string HotelName { get; init; }
        public string Reason { get; init; }
    }
}
