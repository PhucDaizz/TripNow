using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.HotelImage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.HotelImage.Queries
{
    public class GetHotelImagesQueryHandler : IRequestHandler<GetHotelImagesQuery, Result<List<HotelImageDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetHotelImagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<HotelImageDto>>> Handle(GetHotelImagesQuery request, CancellationToken token)
        {
            var hotelExists = await _context.Hotel.AnyAsync(h => h.Id == request.HotelId, token);
            if (!hotelExists)
            {
                return Result.Failure<List<HotelImageDto>>(new Error("Hotel.NotFound", "Không tìm thấy khách sạn."));
            }

            var images = await _context.HotelImage
                .AsNoTracking()
                .Where(img => img.HotelId == request.HotelId) 
                .OrderByDescending(img => img.IsThumbnail)    
                .ThenBy(img => img.CreatedAt)                 
                .Select(img => new HotelImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl, 
                    IsThumbnail = img.IsThumbnail, 
                    Caption = img.Caption 
                })
                .ToListAsync(token);

            return Result.Success(images);
        }
    }
}
