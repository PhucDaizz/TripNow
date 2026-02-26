using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetailBySlug
{
    public class GetHotelDetailBySlugQueryHandler : IRequestHandler<GetHotelDetailBySlugQuery, Result<HotelDetailDto>>
    {
        private readonly IApplicationDbContext _context; 

        public GetHotelDetailBySlugQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<HotelDetailDto>> Handle(GetHotelDetailBySlugQuery request, CancellationToken token)
        {
            var hotel = await _context.Hotel
                .AsNoTracking()
                .Include(x => x.Images)
                .FirstOrDefaultAsync(h => h.Slug == request.Slug, token);

            if (hotel == null)
                return Result.Failure<HotelDetailDto>(new Error("Hotel.NotFound", "Can not found this hotel."));

            var dto = new HotelDetailDto
            {
                Id = hotel.Id,
                OwnerId = hotel.OwnerId,
                Name = hotel.Name,
                Follower = hotel.Follower,
                Slug = hotel.Slug,
                Description = hotel.Description,
                AddressStreet = hotel.Address.Street,
                AddressCity = hotel.Address.City,
                Status = hotel.Status.ToString(),
                Rating = hotel.Rating,
                Location = hotel.Location,
                DistanceKm = null,
                Thumbnail = hotel.Images
                        .Where(i => i.IsThumbnail)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
            };

            return Result.Success(dto);
        }
    }
}
