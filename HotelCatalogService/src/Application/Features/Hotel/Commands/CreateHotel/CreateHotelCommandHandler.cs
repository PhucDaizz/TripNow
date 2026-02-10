using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.ValueObject;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CreateHotel
{
    public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateHotelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            var address = new Address(request.Street, request.City, request.Country);
            var location = new Coordinates(request.Latitude, request.Longitude);

            var hotel = Domain.Entities.Hotel.Create(
                request.OwnerId,
                request.Name,
                request.Description,
                address,
                location,
                request.Rating
            );
            
            await _unitOfWork.Hotel.AddAsync(hotel, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(hotel.Id);
        }
    }
}
