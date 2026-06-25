using Application.DTOs.User;
using Application.Features.User.Queries.GetUsersWithPagination;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IAuthRepository
    {
        Task<UserIdentityDto?> FindByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<ExtendedIdentityUser?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ExtendedIdentityUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<ExtendedIdentityUser>> GetUsersAsync(CancellationToken cancellationToken = default);
        Task UpdateUserAsync(ExtendedIdentityUser user, CancellationToken cancellationToken = default);
        Task<IList<string>> GetUserIdsByRoleIdAsync(string roleId);
        Task<Dictionary<string, string>> GetRolesByUserIdsAsync(List<string> userIds, CancellationToken cancellationToken = default);
        Task<string> GetUserRoleId(string RoleName, CancellationToken cancellationToken = default);
        Task<(List<ExtendedIdentityUser> Users, int TotalCount)> GetFilteredUsersWithPaginationAsync(
            GetUsersWithPaginationQuery request,
            IList<string>? userIdsInRole, 
            CancellationToken cancellationToken = default);
    }
}
