using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Locations;

namespace SocialService.Application.Features.Locations.Queries.GetLocationById
{
    public class GetLocationByIdQuery : IRequest<Result<LocationDto>>
    {
        public Guid Id { get; set; }
    }
}
