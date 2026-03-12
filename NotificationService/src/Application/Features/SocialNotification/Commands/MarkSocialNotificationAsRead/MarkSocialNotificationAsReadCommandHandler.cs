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

            if (notification != null)
            {
                if (notification.OwnerId != request.UserId)
                {
                    return Result.Failure(new Error("Unauthorized", "You are not authorized to mark this notification as read."));
                }
                notification.MarkAsRead();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                int unreadCount = await _unitOfWork.SocialNotification.CountUnreadByUserIdAsync(request.UserId, cancellationToken);

                await _notificationService.UpdateSocialBadgeCountAsync(request.UserId, unreadCount);
                return Result.Success();
            }
            else
            {
                return Result.Failure(new Error("Not Found", "The specified notification was not found."));
            }
        }
    }
}
