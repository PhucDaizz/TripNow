using Domain.Common.Response;
using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application.Features.SocialNotification.Commands.MarkAllSocialNotificationsAsRead
{
    public class MarkAllSocialNotificationsAsReadCommandHandler : IRequestHandler<MarkAllSocialNotificationsAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MarkAllSocialNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(MarkAllSocialNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.SocialNotification.MarkAllAsReadByUserIdAsync(request.UserId, cancellationToken);

            await _notificationService.UpdateSocialBadgeCountAsync(request.UserId, 0);

            return Result.Success();
        }
    }
}
