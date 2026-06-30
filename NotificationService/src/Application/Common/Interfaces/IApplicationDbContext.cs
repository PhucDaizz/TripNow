using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<Notification> NotificationsQuery { get; }
        IQueryable<SocialNotification> SocialNotificationsQuery { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
