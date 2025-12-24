using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.DeleteAmenity
{
    public record DeleteAmenityCommand(Guid Id) : IRequest<Result>;
}
