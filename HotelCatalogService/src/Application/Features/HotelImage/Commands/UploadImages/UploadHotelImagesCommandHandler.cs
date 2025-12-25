using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelImage.Commands.UploadImages
{
    public class UploadHotelImagesCommandHandler : IRequestHandler<UploadHotelImagesCommand, Result<List<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;

        public UploadHotelImagesCommandHandler(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            IImageProcessor imageProcessor)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
        }

        public async Task<Result<List<string>>> Handle(UploadHotelImagesCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Images);
            if (hotel == null) return Result.Failure<List<string>>(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<List<string>>(new Error("Hotel.Forbidden", "No permission"));

            if (request.ImageFiles == null || !request.ImageFiles.Any())
                return Result.Failure<List<string>>(new Error("Images.Empty", "No images selected"));

            var uploadedUrls = new List<string>();

            foreach (var file in request.ImageFiles)
            {
                if (file.Length == 0) continue;

                using var stream = file.OpenReadStream();

                if (!await _imageProcessor.IsValidImageAsync(stream, token)) continue;

                stream.Position = 0;
                using var resizedStream = await _imageProcessor.ResizeAsync(stream, 1920, 1080, "webp", 80, ImageResizeMode.Max, token);

                string folder = $"hotels/{request.HotelId}";

                var originalExt = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{originalExt}";

                var uploadResult = await _cloudinaryService.UploadAsync(resizedStream, fileName, folder, cancellationToken: token);

                if (uploadResult != null)
                {
                    bool isThumb = hotel.Images.Count == 0 && uploadedUrls.Count == 0;

                    hotel.AddImage(uploadResult.Url, uploadResult.PublicId, false, null);
                    uploadedUrls.Add(uploadResult.Url);
                }
            }

            if (uploadedUrls.Count > 0)
            {
                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success(uploadedUrls);
            }

            return Result.Failure<List<string>>(new Error("Upload.None", "No images were uploaded successfully"));
        }
    }
}