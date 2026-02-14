using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Comment
{
    public record CommentHiddenByAdminEvent(Guid commentId, Guid postId, string reason): DomainEvent;
}
