using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Commands.FollowHotel
{
    public class FollowHotelCommandHandler : IRequestHandler<FollowHotelCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly IIntegrationEventService _integrationEventService;

        public FollowHotelCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IHotelCatalogService hotelCatalogService, IIntegrationEventService integrationEventService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _hotelCatalogService = hotelCatalogService;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result<bool>> Handle(FollowHotelCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                return Result.Failure<bool>(new Error("NOT.LOGIN", "User not logged in"));

            var targetUser = await _hotelCatalogService.IsHotelExisting(request.HotelId, cancellationToken);

            if (!targetUser)
                return Result.Failure<bool>(new Error("NOT.FOUND", "User does not exist"));

            var entity = new Domain.Entities.UserFollow(
                currentUserId,
                request.HotelId,
                TypeFollow.FollowHotel
            );

            var eventRequest = new FollowHotelRequest
            {
                HotelId = request.HotelId,
            };

            await _integrationEventService.PublishAsync<FollowHotelRequest>(
                eventRequest,
                "social.events",
                "topic",
                "increase.follow.hotel",
                cancellationToken);

            await _unitOfWork.userFollowRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(true);
        }
    }
}
