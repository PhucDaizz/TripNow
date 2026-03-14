using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.PostLike.Event;
using SocialService.Domain.Enum;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.PostLike;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class PostUnlikedEventHandler : INotificationHandler<PostUnlikedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public PostUnlikedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(PostUnlikedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.PostId);

            if (post == null) return;

            post.DecrementLikeCount();
            await _unitOfWork.postRepository.UpdateAsync(post);

            bool isHotelNotification = post.AuthorType == AuthorType.Hotel;

            await _integrationEventService.PublishAsync<PostUnlikedIntegrationEvent>(
                new PostUnlikedIntegrationEvent
                {
                    OwnerId = post.AuthorId,
                    SocialActionType = SocialActionType.Like,
                    ReferenceId = post.Id,
                    UnlikedUserId = notification.UserId,
                    IsHotelNotification = isHotelNotification 
                },
                "social.events",
                "topic",
                "post.unlike",
                cancellationToken);
        }
    }
}
