using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.SavedPost.Commands.UnsavePost
{
    public class UnsavePostCommandHandler : IRequestHandler<UnsavePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UnsavePostCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(UnsavePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var savedPost = await _unitOfWork.savedPostRepository.IsUserSavePost(userId, request.PostId, cancellationToken);

            if (savedPost == null)
            {
                return Result.Failure<bool>(new Error("SAVEPOST.NOTFOUND", "You haven’t saved this post."));
            }

            await _unitOfWork.savedPostRepository.DeleteAsync(savedPost);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
