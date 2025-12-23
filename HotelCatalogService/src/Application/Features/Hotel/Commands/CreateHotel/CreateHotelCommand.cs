using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CreateHotel
{
    public record CreateHotelCommand: IRequest<Result<Guid>>
    {
        public Guid OwnerId { get; init; }
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string Street { get; init; } = default!;
        public string City { get; init; } = default!;
        public string Country { get; init; } = default!;

        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
