using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Events.Comment;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class CommentCreatedEventHandler : INotificationHandler<CommentCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.postId);

            if (post != null)
            {
                post.IncrementCommentCount();
            }
        }
    }
}
