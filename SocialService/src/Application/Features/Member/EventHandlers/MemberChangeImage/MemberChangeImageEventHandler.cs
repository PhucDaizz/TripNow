using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Member.EventHandlers.MemberChangeImage
{
    public class MemberChangeImageEventHandler : INotificationHandler<MemberChangeImageEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberChangeImageEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MemberChangeImageEvent notification, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.memberRepository.GetByIdAsync(notification.UserId, cancellationToken);
            if (member != null)
            {
                member.UpdateInfo(member.FullName, notification.ImageUrl);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        } 
    }
}
