using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelImage.Commands.DeleteHotelImage
{
    public class DeleteHotelImageCommandHandler : IRequestHandler<DeleteHotelImageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public DeleteHotelImageCommandHandler(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result> Handle(DeleteHotelImageCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Images);

            if (hotel == null)
                return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));

            if (hotel.OwnerId != request.OwnerId)
                return Result.Failure(new Error("Hotel.Forbidden", "You don't have permission to modify this hotel"));

            var imageToDelete = hotel.Images.FirstOrDefault(x => x.Id == request.ImageId);

            if (imageToDelete == null)
                return Result.Failure(new Error("Image.NotFound", "Image does not exist or has been deleted"));

            if (!string.IsNullOrEmpty(imageToDelete.PublicId))
            {
                await _cloudinaryService.DeleteAsync(imageToDelete.PublicId, token);
            }

            hotel.RemoveImage(request.ImageId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}