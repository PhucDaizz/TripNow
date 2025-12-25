using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Commands.DeleteRoomTypeImage
{
    public class DeleteRoomTypeImageCommandHandler : IRequestHandler<DeleteRoomTypeImageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public DeleteRoomTypeImageCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result> Handle(DeleteRoomTypeImageCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdWithDetailsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<List<string>>(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<List<string>>(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "Room type does not exist"));

            var img = roomType.Images.FirstOrDefault(x => x.Id == request.ImageId);
            if (img == null) return Result.Failure(new Error("Image.NotFound", "Image does not exist"));

            if (!string.IsNullOrEmpty(img.PublicId))
            {
                await _cloudinaryService.DeleteAsync(img.PublicId, token);
            }

            roomType.RemoveImage(request.ImageId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
