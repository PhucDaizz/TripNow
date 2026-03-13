using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Member.EventHandlers.HotelThumbnailChanged
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
            var existingMember = await _unitOfWork.memberRepository.GetByIdAsync(notification.HotelId);

            if (existingMember == null) return;

            existingMember.UpdateAvatar(notification.ImageUrl);

            await _unitOfWork.memberRepository.UpdateAsync(existingMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
