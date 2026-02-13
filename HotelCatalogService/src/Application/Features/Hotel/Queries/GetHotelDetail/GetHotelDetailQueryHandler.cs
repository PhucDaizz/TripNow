using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetail
{
    public class GetHotelDetailQueryHandler : IRequestHandler<GetHotelDetailQuery, Result<HotelDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetHotelDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<HotelDetailDto>> Handle(GetHotelDetailQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, cancellationToken, x => x.Images);
            if (hotel == null)
            {
                return Result.Failure<HotelDetailDto>(new Error("NOT.FOUND", "Can not found this hotel"));
            }

            var result = new HotelDetailDto
            {
                Id = request.HotelId,
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

            return Result.Success<HotelDetailDto>(result);
        }
    }
}
