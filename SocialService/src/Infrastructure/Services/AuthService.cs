using Grpc.Core;
using SocialService.Application.Contracts;
using SocialService.Infrastructure.Protos;

public class AuthService : IAuthService
{
    private readonly AuthGrpc.AuthGrpcClient _grpcClient;

    public AuthService(AuthGrpc.AuthGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<bool> IsUserExisting(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new IsUserExistingRequest
            {
                UserId = userId.ToString() 
            };

            var response = await _grpcClient.IsUserExistingAsync(request, cancellationToken: cancellationToken);

            return response.IsExisting;
        }
        catch (RpcException)
        {
            return false;
        }
    }
}