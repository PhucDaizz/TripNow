using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Repositories;

namespace NotificationService.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IApplicationDbContext _context;

        public NotificationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification, CancellationToken token = default)
        {
            await _context.Notifications.AddAsync(notification, token);
        }

        public async Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken token = default)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, token);
        }

        public Task DeleteAsync(Notification notification, CancellationToken token = default)
        {
            _context.Notifications.Remove(notification);
            return Task.CompletedTask;
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, token);
        }

        public async Task MarkAllAsReadByUserIdAsync(Guid userId, CancellationToken token = default)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead == false)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                    token);
        }
    }
}
