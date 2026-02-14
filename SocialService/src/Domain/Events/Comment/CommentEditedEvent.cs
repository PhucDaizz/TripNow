using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Comment
{
    public record CommentEditedEvent(Guid id, Guid postId, string content): DomainEvent;
}
