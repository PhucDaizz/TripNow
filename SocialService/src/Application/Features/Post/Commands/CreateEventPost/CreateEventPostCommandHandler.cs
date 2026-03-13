using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Commands.CreateEventPost
{
    public class CreateEventPostCommandHandler : IRequestHandler<CreateEventPostCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly IAuthorIdentityService _authorIdentityService;

        public CreateEventPostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IHotelCatalogService hotelCatalogService,
            IAuthorIdentityService authorIdentityService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _hotelCatalogService = hotelCatalogService;
            _authorIdentityService = authorIdentityService;
        }

        public async Task<Result<Guid>> Handle(CreateEventPostCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);

            var isHotelExist = await _hotelCatalogService.IsHotelExisting(request.HotelId, cancellationToken);
            if (!isHotelExist)
            {
                return Result.Failure<Guid>(new Error("NOT.FOUND", $"Can not find this hotelId: {request.HotelId}"));
            }

            var authorType = await _authorIdentityService.ResolveAuthorTypeAsync(request.HotelId, cancellationToken);

            if (authorType != AuthorType.Hotel)
            {
                return Result.Failure<Guid>(new Error("FORBIDDEN", "You don't have permission to create an event for this hotel."));
            }

            var post = Domain.Entities.Post.CreateEventPost(request.HotelId, request.HotelId, request.Content);
            post.ChangeCreateBy(currentUserId);

            if (request.Images != null && request.Images.Any())
            {
                for (int i = 0; i < request.Images.Count; i++)
                {
                    var fileDto = request.Images[i];

                    if (fileDto.Length > 0 && fileDto.Content != null)
                    {
                        var originalExtension = System.IO.Path.GetExtension(fileDto.FileName) ?? ".jpg";
                        var fileName = $"event_{post.Id}_{Guid.NewGuid()}{originalExtension}";

                        if (fileDto.Content.CanSeek) fileDto.Content.Position = 0;

                        var uploadResult = await _cloudinaryService.UploadAsync(
                            fileDto.Content,
                            fileName,
                            folder: "social_events",
                            cancellationToken: cancellationToken);

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
