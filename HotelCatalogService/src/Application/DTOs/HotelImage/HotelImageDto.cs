namespace HotelCatalogService.Application.DTOs.HotelImage
{
    public record HotelImageDto
    {
        public string Url { get; init; } = default!;
        public bool IsThumbnail { get; init; }
        public string? Caption { get; init; }
    }
}
