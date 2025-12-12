using Application.DTOs.User;
using System.Security.Claims;

namespace Application.Contracts
{
    public interface ITokenGenerator
    {
        string CreateToken(CreateTokenDTO user, List<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
