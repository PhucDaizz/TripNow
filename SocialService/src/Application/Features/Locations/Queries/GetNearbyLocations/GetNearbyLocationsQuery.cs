using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Locations;

namespace SocialService.Application.Features.Locations.Queries.GetNearbyLocations
{
    public class GetNearbyLocationsQuery : IRequest<Result<List<LocationNearbyDto>>>
    {
        public double UserLatitude { get; set; } 
        public double UserLongitude { get; set; }
        public double RadiusInKm { get; set; } = 5;
    }
}
