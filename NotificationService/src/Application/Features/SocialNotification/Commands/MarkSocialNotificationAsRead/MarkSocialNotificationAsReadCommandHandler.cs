using Domain.Common.Response;
using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SocialNotification.Commands.MarkSocialNotificationAsRead
{
    public class MarkSocialNotificationAsReadCommandHandler : IRequestHandler<MarkSocialNotificationAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MarkSocialNotificationAsReadCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(MarkSocialNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.SocialNotification.GetByIdAsync(request.NotificationId, cancellationToken);

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

            int unreadCount = await _unitOfWork.SocialNotification.CountUnreadByUserIdAsync(request.OwnerId, cancellationToken);

            if (request.IsHotel)
            {
                await _notificationService.UpdateHotelSocialBadgeCountAsync(request.OwnerId, unreadCount);
            }
            else
            {
                await _notificationService.UpdateSocialBadgeCountAsync(request.OwnerId, unreadCount);
            }

            return Result.Success();
        }
    }
}
