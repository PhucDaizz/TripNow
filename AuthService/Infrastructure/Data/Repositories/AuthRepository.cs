using Application.DTOs.User;
using Application.Features.User.Queries.GetUsersWithPagination;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserIdentityDto?> FindByEmailAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null ? new UserIdentityDto
            {
                Id = user.Id,
                Email = user.Email,
            } : null;
        }

        public async Task<(List<ExtendedIdentityUser> Users, int TotalCount)> GetFilteredUsersWithPaginationAsync(GetUsersWithPaginationQuery request, IList<string>? userIdsInRole, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Users
                .Include(u => u.StaffProfile)
                .AsNoTracking()
                .AsQueryable();
            
            if (request.HotelId.HasValue)
            {
                query = query.Where(u => u.StaffProfile != null && u.StaffProfile.HotelId == request.HotelId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    u.UserName.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (request.Gender.HasValue)
            {
                query = query.Where(u => u.Gender == request.Gender.Value);
            }

            if (userIdsInRole != null && userIdsInRole.Any())
            {
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            query = query.OrderBy(u => u.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var pagedUsers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return (pagedUsers, totalCount);
        }

        public async Task<Dictionary<string, string>> GetRolesByUserIdsAsync(List<string> userIds, CancellationToken cancellationToken = default)
        {
            if (userIds == null || !userIds.Any())
            {
                return new Dictionary<string, string>();
            }

            var userRoles = await _dbContext.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_dbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, RoleName = r.Name })
                .ToDictionaryAsync(x => x.UserId, x => x.RoleName, cancellationToken);

            return userRoles;
        }
        public async Task<ExtendedIdentityUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<ExtendedIdentityUser?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IList<string>> GetUserIdsByRoleIdAsync(string roleId)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();
        }

        public async Task<string> GetUserRoleId(string RoleName, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.Roles
                .Select(r => new { r.Id, r.Name })  
                .FirstOrDefaultAsync(x => x.Name == RoleName, cancellationToken);
            return role?.Id ?? string.Empty;
        }

        public Task<List<ExtendedIdentityUser>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserAsync(ExtendedIdentityUser user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}
