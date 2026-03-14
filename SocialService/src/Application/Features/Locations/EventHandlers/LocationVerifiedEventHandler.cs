using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Common;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.Location;

namespace SocialService.Application.Features.Locations.EventHandlers
{
    public class LocationVerifiedEventHandler : INotificationHandler<LocationVerifiedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public LocationVerifiedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(LocationVerifiedEvent notification, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.locationRepository.GetByIdAsync(notification.LocationId);

            if (location == null)
            {
                return; 
            }

            var systemEvent = new SystemNotificationCreateEvent
            {
                OwnerId = notification.CreateBy,
                Title = "Địa điểm đã được phê duyệt! 🎉",
                Message = $"Tuyệt vời! Địa điểm '{location.Name}' của bạn đã được hệ thống kiểm duyệt và hiển thị công khai.",
                Type = NotificationType.LocationVerified, 
                ReferenceId = location.Id.ToString(),
                IsHotelNotification = false
            };

            await _integrationEventService.PublishAsync(
                systemEvent,
                "system.events",               
                "topic",
                "new.notification.system",     
                cancellationToken
            );
        }
    }
}
