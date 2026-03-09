using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
