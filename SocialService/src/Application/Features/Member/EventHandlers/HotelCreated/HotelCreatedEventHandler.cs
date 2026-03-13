using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Member.EventHandlers.HotelCreated
{
    public class HotelCreatedEventHandler : INotificationHandler<HotelCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(HotelCreatedEvent notification, CancellationToken cancellationToken)
        {
            var existingMember = await _unitOfWork.memberRepository.GetByIdAsync(notification.HotelId);
            if (existingMember != null) return;

            var newHotelMember = new Domain.Entities.Member(
                notification.HotelId,
                notification.Name,
                notification.HotelId.ToString() ?? string.Empty,
                AuthorType.Hotel 
            );

            await _unitOfWork.memberRepository.AddAsync(newHotelMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
