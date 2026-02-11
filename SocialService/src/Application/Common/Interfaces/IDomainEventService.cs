using SocialService.Domain.Common;

namespace SocialService.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
