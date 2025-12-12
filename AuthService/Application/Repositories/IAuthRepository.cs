using Application.DTOs.User;

namespace Application.Repositories
{
    public interface IAuthRepository
    {
        Task<UserIdentityDto?> FindByEmailAsync(string email);
        Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateExternalUserAsync(ExternalAuthCommand command);
        Task AssignRoleAsync(string userId, string role);
        Task<bool> HasLoginAsync(string userId, string loginProvider);
        Task AddLoginAsync(string userId, string loginProvider, string providerKey);
        Task<List<string>> GetRolesAsync(string userId);
        Task UpdateRefreshTokenAsync(string userId, string refreshToken);
    }
}
