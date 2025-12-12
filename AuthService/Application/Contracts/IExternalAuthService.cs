using Application.DTOs.User;


namespace Application.Contracts
{
    public interface IExternalAuthService
    {
        Task<AuthResultDto> AuthenticateAsync(ExternalAuthCommand command);
    }
}
