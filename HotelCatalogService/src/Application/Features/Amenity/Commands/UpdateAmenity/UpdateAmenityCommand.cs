using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.UpdateAmenity
{
    public record UpdateAmenityCommand(Guid Id, string Name, string Icon) : IRequest<Result>;
}
