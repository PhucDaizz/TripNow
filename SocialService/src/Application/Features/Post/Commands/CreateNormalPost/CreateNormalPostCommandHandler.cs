using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Application.DTOs.Common;

namespace SocialService.Application.Features.Post.Commands.CreateNormalPost
{
    public class CreateNormalPostCommandHandler : IRequestHandler<CreateNormalPostCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;

        public CreateNormalPostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IImageProcessor imageProcessor)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
        }

        public async Task<Result<Guid>> Handle(CreateNormalPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var post = Domain.Entities.Post.CreateNormalPost(userId, request.HotelId, request.Content);

            if (request.Images != null && request.Images.Any())
            {
                if (request.Images.Count > 10)
                    return Result.Failure<Guid>(new Error("TOO.MANY.PHOTOS", "You can only upload up to 10 images."));

                for (int i = 0; i < request.Images.Count; i++)
                {
                    var file = request.Images[i];
                    if (file.Length > 0)
                    {
                        var fileName = $"post_{post.Id}_{Guid.NewGuid()}";

                        if (file.Content.CanSeek) file.Content.Position = 0;

                        var processedStream = await _imageProcessor.ResizeAsync(
                            imageStream: file.Content,
                            width: 1920,
                            height: 1080,
                            format: "webp",
                            quality: 80,
                            mode: ImageResizeMode.Max, 
                            cancellationToken: cancellationToken
                        );

                        var uploadResult = await _cloudinaryService.UploadAsync(
                            processedStream,
                            fileName,
                            folder: "social_posts", 
                            cancellationToken: cancellationToken
                        );

                        string? caption = (request.ImageCaptions != null && request.ImageCaptions.Count > i)
                                          ? request.ImageCaptions[i]
                                          : null;

                        post.AddImage(
                            uploadResult.Url,
                            uploadResult.PublicId,
                            uploadResult.Width,
                            uploadResult.Height,
                            caption
                        );
                    }
                }
            }

            await _unitOfWork.postRepository.AddAsync(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(post.Id);
        }
    }
}
