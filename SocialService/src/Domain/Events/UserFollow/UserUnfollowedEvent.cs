using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Domain.Events.UserFollow
{
    public record UserUnfollowedEvent(Guid FollowerId, Guid TargetId, TypeFollow Type) : DomainEvent;
}
