using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Locations.Queries.GetPendingLocations
{
    public class GetPendingLocationsQuery : IRequest<Result<PagedResult<PendingLocationDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
