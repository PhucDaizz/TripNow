using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomType.Commands.UpdateRoomType
{
    public class UpdateRoomTypeCommandHandler : IRequestHandler<UpdateRoomTypeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateRoomTypeCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.RoomTypes);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "Room type does not exist"));

            roomType.UpdateDetails(request.Name, request.BasePrice, request.Capacity, request.SizeM2);

            if (hotel.StartingPrice == 0 || request.BasePrice < hotel.StartingPrice)
            {
                hotel.UpdateStartingPrice(request.BasePrice);
            }

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}