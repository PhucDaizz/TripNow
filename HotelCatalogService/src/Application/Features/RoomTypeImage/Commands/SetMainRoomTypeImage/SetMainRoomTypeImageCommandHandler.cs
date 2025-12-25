using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Commands.SetMainRoomTypeImage
{
    public class SetMainRoomTypeImageCommandHandler : IRequestHandler<SetMainRoomTypeImageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetMainRoomTypeImageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SetMainRoomTypeImageCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypesAndImagesAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<List<string>>(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<List<string>>(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure<List<string>>(new Error("RoomType.NotFound", "Room type does not exist"));

            roomType.SetMainImage(request.ImageId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
