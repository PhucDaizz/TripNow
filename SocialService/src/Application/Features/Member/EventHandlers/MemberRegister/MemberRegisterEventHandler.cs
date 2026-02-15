using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Member.EventHandlers.MemberRegister
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
            var isMemberExisting = await _unitOfWork.memberRepository.IsExistingAsync(userId);

            if (!isMemberExisting)
            {
                var newMember = new Domain.Entities.Member(userId, notification.FullName, userId.ToString());
                await _unitOfWork.memberRepository.AddAsync(newMember, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
