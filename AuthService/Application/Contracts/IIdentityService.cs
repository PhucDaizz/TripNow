using Application.DTOs.User;
using Application.Features.User.Commands;
using Domain.Common.Response;

namespace Application.Contracts
{
    public interface IIdentityService
    {
        Task<Result<UserIdentityDto>> CreateUserAsync(
            RegisterCommand command,
            CancellationToken cancellationToken = default);
        Task<Result<UserIdentityDto>> CreateExternalUserAsync(ExternalAuthCommand command);
        Task<Result> AssignRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> HasLoginAsync(string userId, string loginProvider);
        Task AddLoginAsync(string userId, string loginProvider, string providerKey);
        Task<List<string>> GetRolesAsync(string userId);
        Task UpdateRefreshTokenAsync(string userId, string refreshToken);
    }
}
