using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Repositories
{
    public interface ISocialNotificationRepository
    {
        Task AddAsync(SocialNotification socialNotification, CancellationToken token = default);
        Task<SocialNotification?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task DeleteAsync(SocialNotification socialNotification, CancellationToken token = default);
    }
}
