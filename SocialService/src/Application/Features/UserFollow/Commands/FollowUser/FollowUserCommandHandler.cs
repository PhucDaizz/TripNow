using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Commands.FollowUser
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public FollowUserCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IAuthService authService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<Result<bool>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                return Result.Failure<bool>(new Error("NOT.LOGIN", "User not logged in"));

            var targetUser = await _authService.IsUserExisting(request.TargetId, cancellationToken);

            if (!targetUser)
                return Result.Failure<bool>(new Error("NOT.FOUND", "User does not exist"));

            var entity = new Domain.Entities.UserFollow(
                currentUserId,
                request.TargetId,
                TypeFollow.FollowUser
            );

            await _unitOfWork.userFollowRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(true);
        }
    }
}
