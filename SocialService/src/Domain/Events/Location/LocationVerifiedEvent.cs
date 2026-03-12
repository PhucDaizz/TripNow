using SocialService.Domain.Common;

namespace SocialService.Domain.Events.Location
{
    public record LocationVerifiedEvent: DomainEvent
    {
        public Guid LocationId { get; set; }
        public Guid CreateBy { get; set; }
    }
}
