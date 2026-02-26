using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;

namespace SocialService.Application.Features.Post.Commands.CreateNormalPost
{
    public class CreateNormalPostCommandHandler : IRequestHandler<CreateNormalPostCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;

        public CreateNormalPostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<Guid>> Handle(CreateNormalPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var post = Domain.Entities.Post.CreateNormalPost(userId, request.Content, request.HotelId);

            if (request.Images != null && request.Images.Any())
            {
                if (request.Images.Count > 10)
                    return Result.Failure<Guid>(new Error("TOO.MANY.PHOTOS", "You can only upload up to 10 images."));

                for (int i = 0; i < request.Images.Count; i++)
                {
                    var file = request.Images[i];
                    if (file.Length > 0)
                    {
                        var originalExtension = Path.GetExtension(file.FileName) ?? ".jpg";
                        var fileName = $"post_{post.Id}_{Guid.NewGuid()}{originalExtension}";

                        if (file.Content.CanSeek) file.Content.Position = 0;

                        var uploadResult = await _cloudinaryService.UploadAsync(
                            file.Content,
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
