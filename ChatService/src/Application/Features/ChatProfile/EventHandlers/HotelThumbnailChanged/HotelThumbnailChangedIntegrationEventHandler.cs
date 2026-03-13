using ChatService.Application.Common.Interfaces;
using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.HotelThumbnailChanged
{
    public class HotelThumbnailChangedIntegrationEventHandler : INotificationHandler<HotelThumbnailChangedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelThumbnailChangedIntegrationEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(HotelThumbnailChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var existingMember = await _unitOfWork.ChatProfile.GetByIdAsync(notification.HotelId);

            if (existingMember == null) return;

            existingMember.UpdateAvatar(notification.ImageUrl);

            await _unitOfWork.ChatProfile.UpdateAsync(existingMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
