using ChatService.Application.Common.Interfaces;
using ChatService.Domain.Enum;
using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.HotelCreated
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
            var existingMember = await _unitOfWork.ChatProfile.GetByIdAsync(notification.HotelId);
            if (existingMember != null) return;

            var newHotelMember = new Domain.Entities.ChatProfile(
                notification.HotelId,
                notification.Name,
                notification.HotelId.ToString() ?? string.Empty,
                AuthorType.Hotel
            );

            await _unitOfWork.ChatProfile.AddAsync(newHotelMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
