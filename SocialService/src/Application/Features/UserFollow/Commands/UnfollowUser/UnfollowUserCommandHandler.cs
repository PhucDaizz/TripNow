using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowUser
{
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIntegrationEventService _integrationEventService;

        public UnfollowUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result<bool>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var userFollow = await _unitOfWork.userFollowRepository.GetByUserAndTargetAsync(userId, request.UserTargetId, TypeFollow.FollowUser, cancellationToken);

            if (userFollow == null)
            {
                return Result.Failure<bool>(new Error("NOT.FOLLOWING.YET", "You have never followed this user"));
            }

            await _integrationEventService.PublishAsync<UnfollowEvent>(
                new UnfollowEvent
                {
                    UserId = userId
                },
                "social.events",
                "topic",
                "unfollow.user",
                cancellationToken
            );

            await _unitOfWork.userFollowRepository.DeleteAsync(userFollow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success<bool>(true);

        }
    }
}
