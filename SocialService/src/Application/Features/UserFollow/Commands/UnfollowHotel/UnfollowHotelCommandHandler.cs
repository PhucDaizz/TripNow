using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowHotel
{
    public class UnfollowHotelCommandHandler : IRequestHandler<UnfollowHotelCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIntegrationEventService _integrationEventService;

        public UnfollowHotelCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result<bool>> Handle(UnfollowHotelCommand notification, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var userFollow = await _unitOfWork.userFollowRepository.GetByUserAndTargetAsync(userId, notification.HotelId, TypeFollow.FollowHotel, cancellationToken);

            if (userFollow == null)
            {
                return Result.Failure<bool>(new Error("NOT.FOLLOWING.YET", "You have never followed this hotel"));
            }

            await _integrationEventService.PublishAsync<UnfollowHotelEvent>(
                new UnfollowHotelEvent
                {
                    HotelId = notification.HotelId
                },
                "social.events",
                "topic",
                "unfollow.hotel",
                cancellationToken
            );

            await _unitOfWork.userFollowRepository.DeleteAsync(userFollow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success<bool>(true);
        }
    }
}
