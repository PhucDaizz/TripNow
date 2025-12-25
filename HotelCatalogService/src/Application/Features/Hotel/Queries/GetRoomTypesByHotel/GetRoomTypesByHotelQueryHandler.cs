using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomType;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetRoomTypesByHotel
{
    public class GetRoomTypesByHotelQueryHandler : IRequestHandler<GetRoomTypesByHotelQuery, Result<List<RoomTypeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomTypesByHotelQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RoomTypeDto>>> Handle(GetRoomTypesByHotelQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypesAndImagesAsync(request.HotelId, cancellationToken);

            if (hotel == null) return Result.Failure<List<RoomTypeDto>>(new Error("Hotel.NotFound", "Not found"));

            var dtos = hotel.RoomTypes.Select(rt => new RoomTypeDto
            {
                Id = rt.Id,
                Name = rt.Name,
                BasePrice = rt.BasePrice,
                Capacity = rt.Capacity,
                SizeM2 = rt.SizeM2,
                MainImage = rt.Images.FirstOrDefault(i => i.IsMainImage)?.ImageUrl
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
