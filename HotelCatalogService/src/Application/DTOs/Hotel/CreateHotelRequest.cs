namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class CreateHotelRequest
    {
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string Street { get; init; } = default!;
        public string City { get; init; } = default!;
        public string Country { get; init; } = default!;
        public decimal Rating { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
