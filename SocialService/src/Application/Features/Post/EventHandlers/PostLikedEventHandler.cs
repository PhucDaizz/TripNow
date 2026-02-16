using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Events.PostLike;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class PostLikedEventHandler : INotificationHandler<PostLikedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostLikedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PostLikedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.PostId, cancellationToken);
            if (post != null)
            {
                post.IncrementLikeCount();
                await _unitOfWork.postRepository.UpdateAsync(post);
            }
        }
    }
}
