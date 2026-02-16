using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Events.Comment;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class CommentDeletedEventHandler : INotificationHandler<CommentDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentDeletedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CommentDeletedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.postId);

            if (post != null)
            {
                post.DecrementCommentCount();
            }
        }
    }
}
