using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowHotel
{
    public class UnfollowHotelCommandHandler : IRequestHandler<UnfollowHotelCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UnfollowHotelCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(UnfollowHotelCommand notification, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var userFollow = await _unitOfWork.userFollowRepository.GetByUserAndTargetAsync(userId, notification.HotelId, TypeFollow.FollowHotel, cancellationToken);

            if (userFollow == null)
            {
                return Result.Failure<bool>(new Error("NOT.FOLLOWING.YET", "You have never followed this hotel"));
            }

            userFollow.Unfollow();

            await _unitOfWork.userFollowRepository.DeleteAsync(userFollow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success<bool>(true);
        }
    }
}
