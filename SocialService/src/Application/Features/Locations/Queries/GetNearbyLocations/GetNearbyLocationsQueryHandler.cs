using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Locations;

namespace SocialService.Application.Features.Locations.Queries.GetNearbyLocations
{
    public class GetNearbyLocationsQueryHandler : IRequestHandler<GetNearbyLocationsQuery, Result<List<LocationNearbyDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetNearbyLocationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<LocationNearbyDto>>> Handle(GetNearbyLocationsQuery request, CancellationToken cancellationToken)
        {
            var latRange = request.RadiusInKm / 111.1; 
            var lonRange = request.RadiusInKm / (111.1 * Math.Cos(DegToRad(request.UserLatitude))); 

            var minLat = request.UserLatitude - latRange;
            var maxLat = request.UserLatitude + latRange;
            var minLon = request.UserLongitude - lonRange;
            var maxLon = request.UserLongitude + lonRange;

            var locations = await _context.Locations
                .AsNoTracking()
                .Where(x => x.Coordinates.Latitude >= minLat && x.Coordinates.Latitude <= maxLat &&
                            x.Coordinates.Longitude >= minLon && x.Coordinates.Longitude <= maxLon)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Address,
                    x.Type,
                    x.AvgRating,
                    x.IsVerify,
                    Lat = x.Coordinates.Latitude,
                    Lon = x.Coordinates.Longitude
                })
                .ToListAsync(cancellationToken);

            var result = locations
                .Select(x => new LocationNearbyDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Type = x.Type.ToString(),
                    AvgRating = x.AvgRating,
                    IsVerify = x.IsVerify,
                    Latitude = x.Lat,
                    Longitude = x.Lon,
                    DistanceInKm = CalculateHaversineDistance(request.UserLatitude, request.UserLongitude, x.Lat, x.Lon)
                })
                .Where(x => x.DistanceInKm <= request.RadiusInKm) 
                .OrderBy(x => x.DistanceInKm) 
                .ToList();

            return Result<List<LocationNearbyDto>>.Success(result);
        }


        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; 
            var dLat = DegToRad(lat2 - lat1);
            var dLon = DegToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegToRad(lat1)) * Math.Cos(DegToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;

            return Math.Round(d, 2); 
        }

        private double DegToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
