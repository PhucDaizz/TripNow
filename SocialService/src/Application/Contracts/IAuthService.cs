namespace SocialService.Application.Contracts
{
    public interface IAuthService
    {
        Task<bool> IsUserExisting(Guid userId, CancellationToken cancellationToken = default);
    }
}
