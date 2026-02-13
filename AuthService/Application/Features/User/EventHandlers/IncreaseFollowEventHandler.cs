using Application.Common.Interfaces;
using Application.DTOs.User.Event;
using MediatR;

namespace Application.Features.User.EventHandlers
{
    public class IncreaseFollowEventHandler : INotificationHandler<IncreaseFollowEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncreaseFollowEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(IncreaseFollowEvent notification, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Auth.GetUserByIdAsync(notification.UserId.ToString(), cancellationToken);

            if (user != null)
            {
                user.IncreaseFollow();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
