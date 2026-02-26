using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.SavedPost.Commands.SavePost
{
    public class SavePostCommandHandler : IRequestHandler<SavePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService; // Lấy User từ Token

        public SavePostCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(SavePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);


            var isPostExists = await _unitOfWork.postRepository.IsPostExisting(request.PostId, cancellationToken);
            if (!isPostExists)
            {
                return Result.Failure<bool>(new Error("POST.NOT_FOUND", "Bài viết không tồn tại hoặc đã bị xóa."));
            }

            var existingSave = await _unitOfWork.savedPostRepository.IsUserSavePost(userId, request.PostId, cancellationToken);

            if (existingSave != null) 
            {
                return Result<bool>.Success(true);
            }

            var savedPost = new Domain.Entities.SavedPost(request.PostId, userId);

            await _unitOfWork.savedPostRepository.AddAsync(savedPost);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);

        }
    }
}
