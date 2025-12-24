using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelAmenity.Commands.AddHotelAmenity
{
    public class AddHotelAmenityCommandHandler : IRequestHandler<AddHotelAmenityCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddHotelAmenityCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddHotelAmenityCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, x => x.Amenities);
            if (hotel == null)
                return Result.Failure(new Error("Hotel.NotFound", "No hotel found."));

            if (hotel.OwnerId != request.OwnerId)
                return Result.Failure(new Error("Hotel.Forbidden", "Not the hotel owner"));

            var amenityExists = await _unitOfWork.Amenity.ExistsAsync(request.AmenityId, token);
            if (!amenityExists)
                return Result.Failure(new Error("Amenity.NotFound", "Amenities do not exist."));

            hotel.AddAmenity(request.AmenityId, request.Description, request.IsFree);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
