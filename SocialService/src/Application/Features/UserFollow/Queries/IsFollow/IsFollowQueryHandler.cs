using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.UserFollow.Queries.IsFollow
{
    public class IsFollowQueryHandler : IRequestHandler<IsFollowQuery, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public IsFollowQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(IsFollowQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var isFollow = await _unitOfWork.userFollowRepository.IsExisting(userId, request.TargetId, request.TypeFollow, cancellationToken);
            if (isFollow)
            {
                return Result.Success(true);
            }
            return Result.Success(false);
        }
    }
}
