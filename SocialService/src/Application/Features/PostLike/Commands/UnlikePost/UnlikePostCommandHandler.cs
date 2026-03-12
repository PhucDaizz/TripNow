using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.PostLike.Commands.UnlikePost
{
    public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UnlikePostCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        

        public async Task<Result<bool>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var like = await _unitOfWork.postLikeRepository.GetByUserIdAndPostAsync(userId, request.PostId, cancellationToken);
            if (like == null) return Result.Failure<bool>(new Error("NOT.LIKE", "You haven't liked this post yet."));

            like.Delete(userId);

            await _unitOfWork.postLikeRepository.DeleteAsync(like);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
