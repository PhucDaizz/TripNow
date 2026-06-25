using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public IQueryable<ExtendedIdentityUser> UserQuery { get; }
        public IQueryable<StaffProfile> StaffProfilesQuery { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
