using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Exceptions;

namespace SocialService.Application.Features.Post.Commands.CreateReviewPost
{
    public class CreateReviewPostCommandHandler : IRequestHandler<CreateReviewPostCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IBookingService _bookingService;


        public CreateReviewPostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IImageProcessor imageProcessor,
            IBookingService bookingService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _imageProcessor = imageProcessor;
            _bookingService = bookingService;
        }

        public async Task<Result<Guid>> Handle(CreateReviewPostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            if (request.BookingId.HasValue) {
                 var isValidBooking = await _bookingService.IsBookingExisting(request.BookingId.Value, userId, cancellationToken);
                 if (!isValidBooking) return Result.Failure<Guid>(new Error("INVALID.BOOKING" ,"The booking code is invalid."));
            }

            try
            {
                var post = Domain.Entities.Post.CreateReviewPost(
                    userId,
                    request.HotelId,
                    request.Content,
                    request.TargetType,
                    request.TargetId,
                    request.Rating,
                    request.BookingId
                );

                if (request.Images != null && request.Images.Any())
                {
                    for (int i = 0; i < request.Images.Count; i++)
                    {
                        var file = request.Images[i];
                        if (file.Length > 0)
                        {
                            var fileName = $"review_{post.Id}_{Guid.NewGuid()}";

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
                                folder: "social_reviews",
                                cancellationToken: cancellationToken
                            );

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
            catch (DomainException ex)
            {
                // Bắt lỗi từ Domain (ví dụ: Rating < 1, thiếu BookingId)
                return Result.Failure<Guid>(new Error("ERROR", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(new Error("ERROR", ex.Message));
            }
        }
    }
}
