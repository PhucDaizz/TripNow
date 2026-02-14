using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Comment
{
    public record CommentDeletedEvent(Guid commentId, Guid postId): DomainEvent;
}
