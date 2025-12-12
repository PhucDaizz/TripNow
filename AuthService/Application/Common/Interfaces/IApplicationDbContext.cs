using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<ExtendedIdentityUser> Users { get; set; }
        public DbSet<StaffProfile> StaffProfile { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
