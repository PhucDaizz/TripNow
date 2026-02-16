using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Comment
{
    public record CommentCreatedEvent(Guid commentId, Guid postId, Guid userId): DomainEvent;
}
