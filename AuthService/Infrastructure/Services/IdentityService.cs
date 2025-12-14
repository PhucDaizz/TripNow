using Application.Contracts;
using Application.DTOs.User;
using Application.Features.User.Commands.Register;
using Domain.Common.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ExtendedIdentityUser> _userManager;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<ExtendedIdentityUser> userManager,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /*public async Task<Result<ExtendedIdentityUser>> CreateUserAsync(
            string email,
            string password,
            string fullName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = new ExtendedIdentityUser
                {
                    UserName =  email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}");
                    _logger.LogWarning("Failed to create user {Email}: {Errors}",
                        email, string.Join(", ", errors));

                    return Result.Failure<ExtendedIdentityUser>(
                        new Error("Identity.CreateFailed",
                        string.Join("; ", errors)));
                }

                _logger.LogInformation("User created successfully: {Email}", email);
                return Result.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", email);
                return Result.Failure<ExtendedIdentityUser>(
                    new Error("System.Error", "Failed to create user"));
            }
        }
*/
        
        public async Task<Result> AssignRoleAsync(
            string userId,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(new Error("User.NotFound", "User not found"));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return Result.Failure(new Error("Role.AssignFailed",
                    string.Join("; ", result.Errors.Select(e => e.Description))));
            }

            return Result.Success();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return string.Empty;
            }

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<Result> ConfirmEmailAsync(
            string userId,
            string token,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(new Error("User.NotFound", "User not found"));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return Result.Failure(new Error("Email.ConfirmFailed",
                    string.Join("; ", result.Errors.Select(e => e.Description))));
            }

            return Result.Success();
        }

        public Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasLoginAsync(string userId, string loginProvider)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var logins = await _userManager.GetLoginsAsync(user);
            return logins.Any(l => l.LoginProvider == loginProvider);
        }

        public async Task AddLoginAsync(string userId, string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, loginProvider));
            }
        }

        public async Task<List<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task UpdateRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(12);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<Result<UserIdentityDto>> CreateUserAsync(RegisterCommand command, CancellationToken cancellationToken = default)
        {
            var user = new ExtendedIdentityUser
            {
                UserName = command.Email.Trim(),
                FullName = command.FullName.Trim(),
                Email = command.Email.Trim(),
                PhoneNumber = command.PhoneNumber?.Trim(),
            };

            var result = await _userManager.CreateAsync(user, command.Password);

            if (result.Succeeded)
            {
                var userDto = new UserIdentityDto
                {
                    Id = user.Id,
                    Email = user.Email
                };
                return Result.Success(userDto);
            }
            else
            {
                var errors = result.Errors
                    .Select(e => new Error($"Identity.{e.Code}", e.Description))
                    .ToList();

                _logger.LogWarning(
                    "Failed to create user {Email}: {Errors}",
                    command.Email,
                    string.Join(", ", errors.Select(e => e.Message))
                );

                if (errors.Count == 1)
                {
                    return Result.Failure<UserIdentityDto>(errors[0]);
                }

                return Result.Failure<UserIdentityDto>(
                    new Error(
                        "Identity.MultipleErrors",
                        string.Join("; ", errors.Select(e => e.Message))
                    )
                );
            }
        }

        public async Task<Result<UserIdentityDto>> CreateExternalUserAsync(ExternalAuthCommand command)
        {
            var newUser = new ExtendedIdentityUser
            {
                UserName = command.Email,
                Email = command.Email,
                EmailConfirmed = true,
                FullName = $"{command.FirstName} {command.LastName}"
            };

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                var error = result.Errors.Count() == 1
                    ? new Error(
                        $"Identity.{result.Errors.First().Code}",
                        result.Errors.First().Description)
                    : new Error(
                        "Identity.MultipleErrors",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    );

                return Result.Failure<UserIdentityDto>(error);
            }

            var createdUserDto = new UserIdentityDto { Id = newUser.Id, Email = newUser.Email };
            return Result.Success(createdUserDto);
        }

        public async Task<(bool IsAuthenticated, UserIdentityDto? User)> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (false, null);
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
            {
                return (false, null);
            }

            var userDto = new UserIdentityDto
            {
                Id = user.Id,
                Email = user.Email
            };

            return (true, userDto);
        }
    }
}
