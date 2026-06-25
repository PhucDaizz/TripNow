using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomTypeImage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Queries.GetRoomTypeImages
{
    public class GetRoomTypeImagesQueryHandler : IRequestHandler<GetRoomTypeImagesQuery, Result<List<RoomTypeImageDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRoomTypeImagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<RoomTypeImageDto>>> Handle(GetRoomTypeImagesQuery request, CancellationToken token)
        {
            var roomTypeExists = await _context.RoomTypes.AnyAsync(rt => rt.Id == request.RoomTypeId, token);
            if (!roomTypeExists)
            {
                return Result.Failure<List<RoomTypeImageDto>>(new Error("RoomType.NotFound", "Không tìm thấy loại phòng."));
            }

            var images = await _context.RoomTypeImages
                .AsNoTracking()
                .Where(img => img.RoomTypeId == request.RoomTypeId) 
                .OrderByDescending(img => img.IsMainImage)         
                .ThenBy(img => img.CreatedAt)                       
                .Select(img => new RoomTypeImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl, 
                    IsMainImage = img.IsMainImage 
                })
                .ToListAsync(token);

            return Result.Success(images);
        }
    }
}
