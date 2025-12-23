using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.ValueObject;

namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class HotelDto
    {
        public Guid Id { get; init; }
        public Guid OwnerId { get; init; }
        public string Name { get; init; }
        public string Slug { get; init; }
        public string Description { get; init; }
        public string AddressStreet { get; init; }
        public string AddressCity { get; init; }
        public string Status { get; init; }
        public decimal Rating { get; init; }
        public Coordinates Location { get; init; }
        public bool IsActive { get; init; }
        public string Thumbnail { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
