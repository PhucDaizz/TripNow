using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.CreateAmenity
{
    public record CreateAmenityCommand(string Name, string Icon) : IRequest<Result<Guid>>;
}
