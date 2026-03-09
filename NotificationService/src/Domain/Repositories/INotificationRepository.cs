using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken token = default);
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task DeleteAsync(Notification notification, CancellationToken token = default);
        Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken token = default);
        Task MarkAllAsReadByUserIdAsync(Guid userId, CancellationToken token = default);
    }
}
