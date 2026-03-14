using Domain.Common.Response;
using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SystemNotification.Commands.MarkSystemNotificationAsRead
{
    public class MarkSystemNotificationAsReadCommandHandler : IRequestHandler<MarkSystemNotificationAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MarkSystemNotificationAsReadCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(MarkSystemNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Notification.GetByIdAsync(request.NotificationId, cancellationToken);

            if (notification == null)
            {
                return Result.Failure(new Error("Not Found", "The specified notification was not found."));
            }

            if (notification.OwnerId != request.OwnerId)
            {
                return Result.Failure(new Error("Unauthorized", "You are not authorized to mark this notification as read."));
            }

            notification.MarkAsRead();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            int unreadCount = await _unitOfWork.Notification.CountUnreadByUserIdAsync(request.OwnerId, cancellationToken);

            if (request.IsHotel)
            {
                await _notificationService.UpdateHotelSystemBadgeCountAsync(request.OwnerId, unreadCount);
            }
            else
            {
                await _notificationService.UpdateSystemBadgeCountAsync(request.OwnerId, unreadCount);
            }

            return Result.Success();
        }
    }
}
