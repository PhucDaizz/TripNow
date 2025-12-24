
using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotelImage
{
    public class UpdateHotelImageCommandHandler : IRequestHandler<UpdateHotelImageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHotelImageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateHotelImageCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Images);
            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "No permission"));

            hotel.UpdateImageDetails(request.ImageId, request.IsThumbnail, request.Caption);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
