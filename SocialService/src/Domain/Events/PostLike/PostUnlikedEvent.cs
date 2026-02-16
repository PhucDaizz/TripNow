using SocialService.Domain.Common;

namespace SocialService.Domain.Events.PostLike
{
    public record PostUnlikedEvent(Guid PostId) : DomainEvent;
}
