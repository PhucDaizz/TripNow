using MediatR;

namespace RecommendationService.Application.Features.Hotel.EventHandlers.HotelIndexedIntegration
{
    /// <summary>
    /// Integration event nhận từ HotelCatalogService khi một khách sạn được duyệt và publish.
    /// Dữ liệu này sẽ được embedding và lưu vào Qdrant để phục vụ hệ thống đề xuất.
    /// </summary>
    public class HotelIndexedIntegrationEvent : INotification
    {
        public Guid    HotelId       { get; set; }
        public string  Name          { get; set; } = string.Empty;
        public string  Description   { get; set; } = string.Empty;
        public string  City          { get; set; } = string.Empty;
        public string  Street        { get; set; } = string.Empty;
        public string  Country       { get; set; } = string.Empty;
        public decimal Rating        { get; set; }
        public decimal StartingPrice { get; set; }
        public string? ThumbnailUrl  { get; set; }

        public IReadOnlyList<string>                    AmenityNames { get; set; } = [];
        public IReadOnlyList<HotelRoomTypeSummaryDto>   RoomTypes    { get; set; } = [];
    }

    public class HotelRoomTypeSummaryDto
    {
        public string  Name        { get; set; } = string.Empty;
        public decimal BasePrice   { get; set; }
        public int     Capacity    { get; set; }
        public decimal SizeM2      { get; set; }
        public string  Description { get; set; } = string.Empty;
        public string? CancellationPolicyDescription { get; set; }
    }
}
