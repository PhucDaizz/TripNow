using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.PostLike.Event;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.PostLike;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class PostLikedEventHandler : INotificationHandler<PostLikedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public PostLikedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(PostLikedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.PostId, cancellationToken);
            if (post == null) return;

            post.IncrementLikeCount();
            await _unitOfWork.postRepository.UpdateAsync(post);

            if (post.AuthorId == notification.UserId) return;

            var actor = await _unitOfWork.memberRepository.GetByIdAsync(notification.UserId);
            if (actor == null) return;

            await _integrationEventService.PublishAsync<PostLikedIntegrationEvent>(
                new PostLikedIntegrationEvent
                {
                    OwnerId = post.AuthorId,                  
                    SocialActionType = SocialActionType.Like,
                    ReferenceId = post.Id,      
                    LastActorId = actor.Id,
                    LastActorName = actor.FullName ?? "Người dùng",
                    ActorAvatarUrl = actor.AvatarUrl
                },
                "social.events",              
                "topic",                      
                "new.post.like",             
                cancellationToken
            );
        }
    }
}
