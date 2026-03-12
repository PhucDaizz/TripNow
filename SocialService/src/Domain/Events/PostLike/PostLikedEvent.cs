using SocialService.Domain.Common;

namespace SocialService.Domain.Events.PostLike
{
    public record PostLikedEvent(Guid PostId, Guid UserId) : DomainEvent;
}
