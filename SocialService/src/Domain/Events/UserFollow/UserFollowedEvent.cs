using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Domain.Events.UserFollow
{
    public record UserFollowedEvent(Guid FollowerId, Guid TargetId, TypeFollow Type) : DomainEvent;
}
