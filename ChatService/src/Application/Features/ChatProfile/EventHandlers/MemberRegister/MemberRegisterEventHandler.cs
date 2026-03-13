using ChatService.Application.Common.Interfaces;
using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.MemberRegister
{
    public class MemberRegisterEventHandler : INotificationHandler<MemberRegisterEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberRegisterEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MemberRegisterEvent notification, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(notification.UserId);
            var isMemberExisting = await _unitOfWork.ChatProfile.IsExistingAsync(userId);

            if (!isMemberExisting)
            {
                var newMember = new Domain.Entities.ChatProfile(userId, notification.FullName, userId.ToString());
                await _unitOfWork.ChatProfile.AddAsync(newMember, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
