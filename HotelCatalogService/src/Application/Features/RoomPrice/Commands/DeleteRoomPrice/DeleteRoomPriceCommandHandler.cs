using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Commands.DeleteRoomPrice
{
    public class DeleteRoomPriceCommandHandler : IRequestHandler<DeleteRoomPriceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomPriceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteRoomPriceCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypePricesAsync(request.HotelId, request.RoomTypeId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            var roomType = hotel.RoomTypes.FirstOrDefault();
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "RoomType not existing"));

            roomType.RemovePriceForDate(request.Date);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
