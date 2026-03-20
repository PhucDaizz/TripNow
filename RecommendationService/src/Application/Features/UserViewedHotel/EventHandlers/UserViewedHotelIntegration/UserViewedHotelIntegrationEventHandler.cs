using MediatR;
using RecommendationService.Application.Common.Interfaces;

namespace RecommendationService.Application.Features.UserViewedHotel.EventHandlers.UserViewedHotelIntegration
{
    public class UserViewedHotelIntegrationEventHandler : INotificationHandler<UserViewedHotelIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserViewedHotelIntegrationEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UserViewedHotelIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var existingView = await _unitOfWork.UserViewedHotels.GetByUserAndHotelAsync(notification.UserId, notification.HotelId);

            if (existingView != null)
            {
                existingView.UpdateViewTime(notification.ViewedAt);
                _unitOfWork.UserViewedHotels.Update(existingView);
            }
            else
            {
                existingView = Domain.Entities.UserViewedHotel.Create(notification.UserId, notification.HotelId, notification.ViewedAt);
                await _unitOfWork.UserViewedHotels.AddAsync(existingView);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
