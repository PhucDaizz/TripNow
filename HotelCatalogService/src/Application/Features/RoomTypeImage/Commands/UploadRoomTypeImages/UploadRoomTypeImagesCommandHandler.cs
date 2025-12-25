using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Commands.UploadRoomTypeImages
{
    public class UploadRoomTypeImagesCommandHandler : IRequestHandler<UploadRoomTypeImagesCommand, Result<List<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;

        public UploadRoomTypeImagesCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, IImageProcessor imageProcessor)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
        }

        public async Task<Result<List<string>>> Handle(UploadRoomTypeImagesCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypesAndImagesAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<List<string>>(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<List<string>>(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure<List<string>>(new Error("RoomType.NotFound", "Room type does not exist"));

            var uploadedUrls = new List<string>();
            foreach (var file in request.ImageFiles)
            {
                if (file.Length == 0) continue;

                using var stream = file.OpenReadStream();
                if (!await _imageProcessor.IsValidImageAsync(stream, token)) continue;

                stream.Position = 0;
                using var resizedStream = await _imageProcessor.ResizeAsync(stream, 1920, 1080, "webp", 80, ImageResizeMode.Max, token);

                string folder = $"hotels/{request.HotelId}/roomtypes";

                var originalExt = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{originalExt}";

                var result = await _cloudinaryService.UploadAsync(resizedStream, fileName, folder, cancellationToken: token);

                if (result != null)
                {
                    roomType.AddImage(result.Url, result.PublicId);
                    uploadedUrls.Add(result.Url);
                }
            }

            if (uploadedUrls.Count == 0)
                return Result.Failure<List<string>>(new Error("Upload.Failed", "No images were uploaded successfully"));

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(uploadedUrls);
        }
    }
}