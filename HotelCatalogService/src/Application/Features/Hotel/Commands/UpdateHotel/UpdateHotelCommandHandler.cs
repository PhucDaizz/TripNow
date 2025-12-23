using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using HotelCatalogService.Domain.ValueObject;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotel
{
    public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, Result>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHotelCommandHandler(IHotelRepository hotelRepository, IUnitOfWork unitOfWork)
        {
            _hotelRepository = hotelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));
            }

            if (hotel.OwnerId != request.OwnerId)
            {
                return Result.Failure(new Error("Hotel.Forbidden", "You are not the owner of this hotel"));
            }

            var newAddress = new Address(request.Street, request.City, request.Country);
            var newLocation = new Coordinates(request.Latitude, request.Longitude);

            hotel.UpdateInfo(
                request.Name,
                request.Description,
                newAddress,
                newLocation
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
