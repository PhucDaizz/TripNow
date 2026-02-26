using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Locations.Queries.GetPendingLocations
{
    public class GetPendingLocationsQueryHandler : IRequestHandler<GetPendingLocationsQuery, Result<PagedResult<PendingLocationDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingLocationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<PendingLocationDto>>> Handle(GetPendingLocationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Locations
                .AsNoTracking()
                .Where(l => l.IsVerify == false);

            var totalCount = await query.CountAsync(cancellationToken);

            var locations = await query
                .OrderBy(l => l.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new PendingLocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    Type = l.Type.ToString(),
                    AvgRating = l.AvgRating,
                    IsVerify = l.IsVerify,
                    CreatedByUserId = l.CreatedByUserId,
                    CreatedAt = l.CreatedAt,
                    Latitude = l.Coordinates.Latitude,
                    Longitude = l.Coordinates.Longitude
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<PendingLocationDto>.Create(locations, totalCount, request.PageNumber, request.PageSize);

            return Result.Success(pagedResult);
        }
    }
}
