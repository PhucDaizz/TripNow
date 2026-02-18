using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;

namespace SocialService.Application.Features.Post.Commands.CreateEventPost
{
    public class CreateEventPostCommandHandler : IRequestHandler<CreateEventPostCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IHotelCatalogService _hotelCatalogService;

        public CreateEventPostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IImageProcessor imageProcessor, 
            IHotelCatalogService hotelCatalogService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
            _hotelCatalogService = hotelCatalogService;
        }

        public async Task<Result<Guid>> Handle(CreateEventPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var hotel = await _hotelCatalogService.GetHotelDetail(request.HotelId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure<Guid>(new Error("NOT.FOUD", $"Can not find this hotelId: {request.HotelId}"));
            }

            if (userId != hotel.OwnerId)
            {
                return Result.Failure<Guid>(new Error("FORBIDDEN", "You are not the owner of this hotel."));
            }

            var post = Domain.Entities.Post.CreateEventPost(userId, request.HotelId, request.Content);

            if (request.Images != null && request.Images.Any())
            {
                for (int i = 0; i < request.Images.Count; i++)
                {
                    var fileDto = request.Images[i];

                    if (fileDto.Length > 0 && fileDto.Content != null)
                    {
                        var fileName = $"event_{post.Id}_{Guid.NewGuid()}";
                        if (fileDto.Content.CanSeek) fileDto.Content.Position = 0;

                        using var processedStream = await _imageProcessor.ResizeAsync(
                            fileDto.Content, 1920, 1080, "webp", 80, ImageResizeMode.Max, cancellationToken);

                        var uploadResult = await _cloudinaryService.UploadAsync(
                            processedStream, fileName, folder: "social_events", cancellationToken: cancellationToken);

                        string? caption = (request.ImageCaptions != null && request.ImageCaptions.Count > i)
                                          ? request.ImageCaptions[i]
                                          : null;

                        post.AddImage(uploadResult.Url, uploadResult.PublicId, uploadResult.Width, uploadResult.Height, caption);
                    }
                }
            }

            await _unitOfWork.postRepository.AddAsync(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(post.Id);
        }
    }
}
