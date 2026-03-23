using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Events.Hotel
{
    public record HotelPublishedEvent(
        Guid Id,
        string Name,
        string Description,
        string City,
        string Street,
        string Country,
        decimal Rating,
        decimal StartingPrice,
        IReadOnlyList<string> AmenityNames,
        IReadOnlyList<RoomTypeSummary> RoomTypes,
        string? ThumbnailUrl) : DomainEvent;

    public record RoomTypeSummary(
        string Name,
        decimal BasePrice,
        int Capacity,
        decimal SizeM2,
        string Description,
        string? CancellationPolicyDescription);
}
