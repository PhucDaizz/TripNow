using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Locations.Queries.GetUserContributedLocations
{
    public class GetUserContributedLocationsQueryHandler
        : IRequestHandler<GetUserContributedLocationsQuery, Result<PagedResult<ContributedLocationDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserContributedLocationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<ContributedLocationDto>>> Handle(
            GetUserContributedLocationsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Locations
                .AsNoTracking()
                .Where(l => l.CreatedByUserId == request.UserId);

            if (request.Type.HasValue)
            {
                query = query.Where(l => l.Type == request.Type.Value);
            }
            var totalCount = await query.CountAsync(cancellationToken);

            var locations = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new ContributedLocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    Type = l.Type.ToString(),
                    AvgRating = l.AvgRating,
                    IsVerify = l.IsVerify,
                    Latitude = l.Coordinates.Latitude,
                    Longitude = l.Coordinates.Longitude
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<ContributedLocationDto>.Create(
                locations,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(pagedResult);
        }
    }
}
