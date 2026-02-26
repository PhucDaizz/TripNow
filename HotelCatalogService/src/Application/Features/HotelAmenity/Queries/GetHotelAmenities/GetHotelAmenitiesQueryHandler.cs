using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.HotelAmenity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.HotelAmenity.Queries.GetHotelAmenities
{
    public class GetHotelAmenitiesQueryHandler : IRequestHandler<GetHotelAmenitiesQuery, Result<List<HotelAmenityDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetHotelAmenitiesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<HotelAmenityDto>>> Handle(GetHotelAmenitiesQuery request, CancellationToken token)
        {
            var hotelExists = await _context.Hotel.AnyAsync(h => h.Id == request.HotelId, token);
            if (!hotelExists)
            {
                return Result.Failure<List<HotelAmenityDto>>(new Error("Hotel.NotFound", "Hotel is not avaiable."));
            }

            var query = from ha in _context.HotelAmenity.AsNoTracking()
                        join a in _context.Amenity.AsNoTracking()
                            on ha.AmenityId equals a.Id
                        where ha.HotelId == request.HotelId
                        select new HotelAmenityDto
                        {
                            Id = ha.Id, 
                            AmenityId = ha.AmenityId,
                            Name = a.Name, 
                            Icon = a.Icon, 
                            Description = ha.Description,
                            IsFree = ha.IsFree
                        };

            var amenities = await query.ToListAsync(token);

            return Result.Success(amenities);
        }
    }
}
