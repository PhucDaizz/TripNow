using HotelCatalogService.Application.Features.Hotel.Commands.AddHotelStructure;

namespace HotelCatalogService.Application.DTOs.Hotel
{
    public record AddHotelStructure
    {
        public Guid HotelId { get; init; }
        public List<CreateBlockDto> Blocks { get; init; }
    }
}
