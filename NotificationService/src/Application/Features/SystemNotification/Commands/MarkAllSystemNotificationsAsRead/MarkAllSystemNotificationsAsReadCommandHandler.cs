using Domain.Common.Response;
using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SystemNotification.Commands.MarkAllSystemNotificationsAsRead
{
    public class MarkAllSystemNotificationsAsReadCommandHandler : IRequestHandler<MarkAllSystemNotificationsAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MarkAllSystemNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(MarkAllSystemNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Notification.MarkAllAsReadByUserIdAsync(request.OwnerId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.IsHotel)
            {
                await _notificationService.UpdateHotelSystemBadgeCountAsync(request.OwnerId, 0);
            }
            else
            {
                await _notificationService.UpdateSystemBadgeCountAsync(request.OwnerId, 0);
            }

            return Result.Success();
        }
    }
}
