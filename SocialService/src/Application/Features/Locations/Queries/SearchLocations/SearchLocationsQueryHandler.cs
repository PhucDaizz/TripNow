using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Locations;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Locations.Queries.SearchLocations
{
    public class SearchLocationsQueryHandler : IRequestHandler<SearchLocationsQuery, Result<PagedResult<LocationDto>>>
    {
        private readonly IApplicationDbContext _context;

        public SearchLocationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<LocationDto>>> Handle(SearchLocationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Locations.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword) || x.Address.Contains(request.Keyword));
            }

            if (request.Type.HasValue)
            {
                query = query.Where(x => x.Type == request.Type.Value);
            }

            query = query.OrderByDescending(x => x.IsVerify)
                         .ThenByDescending(x => x.AvgRating);

            var resultQuery = query.Select(x => new LocationDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Latitude = x.Coordinates.Latitude,   
                Longitude = x.Coordinates.Longitude, 
                Type = x.Type.ToString(),           
                AvgRating = x.AvgRating,
                IsVerify = x.IsVerify
            });

            var totalCount = await resultQuery.CountAsync(cancellationToken);
            var items = await resultQuery
                                .Skip((request.PageIndex - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync(cancellationToken);

            return Result<PagedResult<LocationDto>>.Success(
                new PagedResult<LocationDto>(items, request.PageIndex, request.PageSize, totalCount));
        }
    }
}
