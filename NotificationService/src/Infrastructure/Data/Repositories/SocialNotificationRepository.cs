using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;
using NotificationService.Domain.Repositories;

namespace NotificationService.Infrastructure.Data.Repositories
{
    public class SocialNotificationRepository : ISocialNotificationRepository
    {
        private readonly IApplicationDbContext _context;

        public SocialNotificationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SocialNotification socialNotification, CancellationToken token = default)
        {
            await _context.SocialNotifications.AddAsync(socialNotification, token);
        }

        public async Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken token = default)
        {
            return await _context.SocialNotifications.CountAsync(n => n.OwnerId == userId && !n.IsRead, token);
        }

        public Task DeleteAsync(SocialNotification socialNotification, CancellationToken token = default)
        {
            _context.SocialNotifications.Remove(socialNotification);
            return Task.CompletedTask; 
        }

        public async Task<SocialNotification?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.SocialNotifications.FirstOrDefaultAsync(n => n.Id == id, token);
        }

        public async Task<SocialNotification?> GetExistingForAggregationAsync(Guid userId, SocialActionType actionType, string referenceId, CancellationToken cancellationToken = default)
        {
            return await _context.SocialNotifications
                .FirstOrDefaultAsync(n =>
                    n.OwnerId == userId &&
                    n.ActionType == actionType &&
                    n.ReferenceId == referenceId,
                    cancellationToken);
        }

        public async Task MarkAllAsReadByUserIdAsync(Guid userId, CancellationToken token = default)
        {
            await _context.SocialNotifications
                .Where(n => n.OwnerId == userId && n.IsRead == false)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                    token);
        }

        public Task RemoveAsync(SocialNotification socialNotification, CancellationToken token = default)
        {
            _context.SocialNotifications.Remove(socialNotification);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(SocialNotification socialNotification, CancellationToken token = default)
        {
            _context.SocialNotifications.Update(socialNotification);
            return Task.CompletedTask;
        }
    }
}
