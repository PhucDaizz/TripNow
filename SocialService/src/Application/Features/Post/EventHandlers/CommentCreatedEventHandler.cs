using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Comment.Event;
using SocialService.Domain.Enum;
using SocialService.Domain.Enum.NotificationService;
using SocialService.Domain.Events.Comment;

namespace SocialService.Application.Features.Post.EventHandlers
{
    public class CommentCreatedEventHandler : INotificationHandler<CommentCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public CommentCreatedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.postRepository.GetByIdAsync(notification.PostId);
            if (post == null) return;

            post.IncrementCommentCount();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var actor = await _unitOfWork.memberRepository.GetByIdAsync(notification.UserId);
            if (actor == null) return;

            Guid receiverId;
            SocialActionType actionType;
            Guid referenceId;
            bool isHotelNotification = false;

            if (notification.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.commentRepository.GetByIdAsync(notification.ParentCommentId.Value);
                if (parentComment == null) return;

                actionType = SocialActionType.ReplyComment;
                referenceId = parentComment.Id;

                if (parentComment.AuthorType == AuthorType.Hotel && post.HotelId.HasValue)
                {
                    receiverId = post.HotelId.Value; 
                    isHotelNotification = true;
                }
                else
                {
                    receiverId = parentComment.UserId; 
                    isHotelNotification = false;
                }
            }
            else
            {
                actionType = SocialActionType.Comment; 
                referenceId= post.Id;

                if (post.AuthorType == AuthorType.Hotel && post.HotelId.HasValue)
                {
                    receiverId = post.HotelId.Value;
                    isHotelNotification = true;
                }
                else
                {
                    receiverId = post.UserId; 
                    isHotelNotification = false;
                }
            }

            if (!isHotelNotification && receiverId == notification.UserId)
            {
                return; 
            }

            await _integrationEventService.PublishAsync<CommentCreateEvent>(
                new CommentCreateEvent
                {
                    OwnerId = receiverId,
                    SocialActionType = actionType,
                    ReferenceId = referenceId,
                    LastActorId = actor.Id,
                    LastActorName = actor.FullName ?? "Người dùng",
                    ActorAvatarUrl = actor.AvatarUrl,
                    IsHotelNotification = isHotelNotification
                },
                "social.events",
                "topic",
                "new.comment.create",
                cancellationToken
            );
        }
    }
}
