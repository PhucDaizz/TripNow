using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.DTOs.SystemNotification;
using NotificationService.Application.DTOs.SystemNotification.Event;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SystemNotification.EventHandlers.CreateSystemNotification
{
    public class CreateSystemNotificationEventHandler : INotificationHandler<SystemNotificationCreateEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CreateSystemNotificationEventHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(SystemNotificationCreateEvent notification, CancellationToken cancellationToken)
        {
            var newNotif = new Domain.Entities.Notification(
                notification.OwnerId,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.ReferenceId
            );

            await _unitOfWork.Notification.AddAsync(newNotif);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var notifDto = SystemNotificationDto.FromEntity(newNotif);

            int unreadCount = await _unitOfWork.Notification.CountUnreadByUserIdAsync(notification.OwnerId, cancellationToken);

            if (notification.IsHotelNotification)
            {
                await _notificationService.SendHotelSystemNotificationAsync(notification.OwnerId, notifDto);
                await _notificationService.UpdateHotelSystemBadgeCountAsync(notification.OwnerId, unreadCount);
            }
            else
            {
                await _notificationService.SendSystemNotificationAsync(notification.OwnerId, notifDto);
                await _notificationService.UpdateSystemBadgeCountAsync(notification.OwnerId, unreadCount);
            }
        }
    }
}
