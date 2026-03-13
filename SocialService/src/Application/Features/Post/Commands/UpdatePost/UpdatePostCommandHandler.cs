using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IAuthorIdentityService _authorIdentityService;

        public UpdatePostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IImageProcessor imageProcessor,
            IAuthorIdentityService authorIdentityService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
            _authorIdentityService = authorIdentityService;
        }

        public async Task<Result<bool>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);

            var post = await _unitOfWork.postRepository.GetPostDetailAsync(request.PostId, cancellationToken);

            if (post == null || post.IsDeleted)
                return Result.Failure<bool>(new Error("NOT_FOUND", "The article does not exist."));

            bool hasPermission = false;

            var currentUserHotelContext = await _authorIdentityService.ResolveAuthorTypeAsync(post.HotelId, cancellationToken);

            if (post.AuthorType == AuthorType.User && post.AuthorId == currentUserId)
            {
                hasPermission = true;
            }
            else if (post.AuthorType == AuthorType.Hotel && currentUserHotelContext == AuthorType.Hotel)
            {
                hasPermission = true;
            }

            if (!hasPermission)
            {
                return Result.Failure<bool>(new Error("FORBIDDEN", "You do not have permission to edit this post."));
            }

            post.UpdateContent(request.Content);

            if (request.DeletedImageIds != null && request.DeletedImageIds.Any())
            {
                foreach (var imgId in request.DeletedImageIds)
                {
                    var publicId = post.GetImagePublicId(imgId);

                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _cloudinaryService.DeleteAsync(publicId, cancellationToken);
                    }

                    post.RemoveImage(imgId);
                }
            }

            if (request.NewImages != null && request.NewImages.Any())
            {
                for (int i = 0; i < request.NewImages.Count; i++)
                {
                    var fileDto = request.NewImages[i];
                    if (fileDto.Length > 0 && fileDto.Content != null)
                    {
                        var fileName = $"post_{post.Id}_{Guid.NewGuid()}";
                        if (fileDto.Content.CanSeek) fileDto.Content.Position = 0;

                        using var processedStream = await _imageProcessor.ResizeAsync(
                             fileDto.Content, 1920, 1080, "webp", 80, ImageResizeMode.Max, cancellationToken);

                        var uploadResult = await _cloudinaryService.UploadAsync(
                            processedStream, fileName, folder: "social_posts", cancellationToken: cancellationToken);

                        string? caption = (request.NewImageCaptions != null && request.NewImageCaptions.Count > i)
                                          ? request.NewImageCaptions[i]
                                          : null;

                        post.AddImage(uploadResult.Url, uploadResult.PublicId, uploadResult.Width, uploadResult.Height, caption);
                    }
                }
            }

            await _unitOfWork.postRepository.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
