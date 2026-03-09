using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
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

        public Task DeleteAsync(SocialNotification socialNotification, CancellationToken token = default)
        {
            _context.SocialNotifications.Remove(socialNotification);
            return Task.CompletedTask; 
        }

        public async Task<SocialNotification?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.SocialNotifications.FirstOrDefaultAsync(n => n.Id == id, token);
        }
    }
}
