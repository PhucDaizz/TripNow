using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Locations.Queries.GetUserContributedLocations
{
    public class GetUserContributedLocationsQuery : IRequest<Result<PagedResult<ContributedLocationDto>>>
    {
        public Guid UserId { get; set; }
        public LocationType? Type { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
