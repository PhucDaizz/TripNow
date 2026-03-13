using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Conversations> Conversations { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<ChatProfile> ChatProfiles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
