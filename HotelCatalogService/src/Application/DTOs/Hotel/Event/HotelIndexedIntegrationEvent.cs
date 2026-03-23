using HotelCatalogService.Application.Features.Hotel.EventHandlers;

namespace HotelCatalogService.Application.DTOs.Hotel.Event
{
    public class HotelIndexedIntegrationEvent
    {
        public Guid HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public decimal StartingPrice { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IReadOnlyList<string> AmenityNames { get; set; } = [];
        public IReadOnlyList<HotelRoomTypeSummary> RoomTypes { get; set; } = [];
    }
}
