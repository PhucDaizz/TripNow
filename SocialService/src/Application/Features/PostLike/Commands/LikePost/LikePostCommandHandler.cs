using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.PostLike.Commands.LikePost
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public LikePostCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var existingLike = await _unitOfWork.postLikeRepository.GetByUserIdAndPostAsync(userId, request.PostId, cancellationToken);
            if (existingLike != null) return Result<bool>.Success(true);

            var newLike = Domain.Entities.PostLike.Create(request.PostId, userId);

            await _unitOfWork.postLikeRepository.AddAsync(newLike, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
