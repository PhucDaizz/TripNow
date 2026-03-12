using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Repositories
{
    public interface ISocialNotificationRepository
    {
        Task AddAsync(SocialNotification socialNotification, CancellationToken token = default);
        Task<SocialNotification?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task UpdateAsync(SocialNotification socialNotification, CancellationToken token = default);
        Task RemoveAsync(SocialNotification socialNotification, CancellationToken token = default);
        Task DeleteAsync(SocialNotification socialNotification, CancellationToken token = default);
        Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken token = default);
        Task MarkAllAsReadByUserIdAsync(Guid userId, CancellationToken token = default);
        Task<SocialNotification?> GetExistingForAggregationAsync(
            Guid userId,
            SocialActionType actionType,
            string referenceId,
            CancellationToken cancellationToken = default);
    }
}
