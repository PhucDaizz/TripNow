using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Locations.Queries.SearchLocations
{
    public class SearchLocationsQuery : IRequest<Result<PagedResult<LocationDto>>>
    {
        public string? Keyword { get; set; }
        public LocationType? Type { get; set; } 
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
}
