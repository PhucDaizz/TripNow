using Application.Common.Interfaces;
using Application.DTOs.User.Event;
using MediatR;

namespace Application.Features.User.EventHandlers
{
    public class UnfollowEventHandler : INotificationHandler<UnfollowEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnfollowEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UnfollowEvent notification, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Auth.GetUserByIdAsync(notification.UserId.ToString(), cancellationToken);

            if (user != null)
            {
                user.UnFollow();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
