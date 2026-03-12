using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Comment
{
    public record CommentCreatedEvent(Guid CommentId, Guid PostId, Guid UserId, Guid? ParentCommentId): DomainEvent;
}
