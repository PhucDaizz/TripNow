using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.DTOs.User;
using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<ExtendedIdentityUser> _userManager;

        public AuthRepository(IApplicationDbContext dbContext, UserManager<ExtendedIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task AddLoginAsync(string userId, string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, loginProvider));
            }
        }

        public async Task AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateExternalUserAsync(ExternalAuthCommand command)
        {
            var userName = StringUtils.Slugify($"{command.FirstName} {command.LastName}");
            var newUser = new ExtendedIdentityUser
            {
                UserName = userName,
                Email = command.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToList(), null);
            }

            var createdUserDto = new UserIdentityDto { Id = newUser.Id, Email = newUser.Email };
            return (true, new List<string>(), createdUserDto);
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

        public async Task<List<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> HasLoginAsync(string userId, string loginProvider)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var logins = await _userManager.GetLoginsAsync(user);
            return logins.Any(l => l.LoginProvider == loginProvider);
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
    }
}
