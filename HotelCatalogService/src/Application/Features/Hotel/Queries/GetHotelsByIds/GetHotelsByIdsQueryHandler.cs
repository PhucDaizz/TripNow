using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsByIds
{
    public class GetHotelsByIdsQueryHandler : IRequestHandler<GetHotelsByIdsQuery, Result<IEnumerable<HotelSummaryDtoo>>>
    {
        private readonly IApplicationDbContext _context;

        public GetHotelsByIdsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<HotelSummaryDtoo>>> Handle(GetHotelsByIdsQuery request, CancellationToken token)
        {
            if (request.HotelIds == null || !request.HotelIds.Any())
                return Result.Success<IEnumerable<HotelSummaryDtoo>>(new List<HotelSummaryDtoo>());

            var hotels = await _context.Hotel
                .AsNoTracking()
                .Include(x => x.Images) 
                .Where(h => request.HotelIds.Contains(h.Id) && h.Status == HotelStatus.Active)
                .ToListAsync(token);

            var dtos = hotels.Select(h => new HotelSummaryDtoo
            {
                Id = h.Id,
                Name = h.Name,
                Slug = h.Slug,
                Rating = h.Rating,
                StartingPrice = h.StartingPrice,
                AddressCity = h.Address.City,
                Thumbnail = h.Images.FirstOrDefault(i => i.IsThumbnail)?.ImageUrl
            }).ToList();

            var sortedDtos = dtos
                .OrderBy(dto => request.HotelIds.IndexOf(dto.Id))
                .ToList();

            return Result.Success<IEnumerable<HotelSummaryDtoo>>(sortedDtos);
        }
    }
}
