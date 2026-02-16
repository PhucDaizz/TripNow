using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Events.PostLike;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class PostUnlikedEventHandler : INotificationHandler<PostUnlikedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostUnlikedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PostUnlikedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.PostId);
            if (post != null)
            {
                post.DecrementLikeCount();
                await _unitOfWork.postRepository.UpdateAsync(post);
            }
        }
    }
}
