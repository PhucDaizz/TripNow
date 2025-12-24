using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelAmenity.Commands.UpdateHotelAmenity
{
    public class UpdateHotelAmenityCommandHandler : IRequestHandler<UpdateHotelAmenityCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateHotelAmenityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateHotelAmenityCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Amenities);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "No hotel found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "You are not allowed"));

            if (!hotel.Amenities.Any(x => x.AmenityId == request.AmenityId))
                return Result.Failure(new Error("HotelAmenity.NotFound", "This amenity is not available in the hotel."));

            hotel.UpdateAmenity(request.AmenityId, request.Description, request.IsFree);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
