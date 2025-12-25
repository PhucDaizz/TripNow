using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.DeleteRoomType
{
    public class DeleteRoomTypeCommandHandler : IRequestHandler<DeleteRoomTypeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public DeleteRoomTypeCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result> Handle(DeleteRoomTypeCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypesAndImagesAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "Room type does not exist"));

            if (roomType.Images != null && roomType.Images.Any())
            {
                var publicIds = roomType.Images
                    .Where(img => !string.IsNullOrEmpty(img.PublicId))
                    .Select(img => img.PublicId)
                    .ToList();

                if (publicIds.Count > 0)
                {
                    await _cloudinaryService.DeleteManyAsync(publicIds, token);
                }
            }

            hotel.RemoveRoomType(request.RoomTypeId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}