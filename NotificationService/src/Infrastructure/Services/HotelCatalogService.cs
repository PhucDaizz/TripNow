using Grpc.Core;
using Microsoft.AspNetCore.Http;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Infrastructure.Protos;

namespace NotificationService.Infrastructure.Services
{
    public class HotelCatalogService : IHotelCatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _grpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelCatalogService(CatalogGrpc.CatalogGrpcClient grpcClient, IHttpContextAccessor httpContextAccessor)
        {
            _grpcClient = grpcClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                var bearerToken = context?.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(bearerToken))
                {
                    return false;
                }

                var headers = new Metadata
                {
                    { "Authorization", bearerToken }
                };

                var request = new VerifyHotelOwnershipRequest
                {
                    HotelId = hotelId.ToString()
                };

                var response = await _grpcClient.VerifyHotelOwnershipAsync(
                    request,
                    headers: headers,
                    cancellationToken: cancellationToken);

                return response.IsOwner;
            }
            catch (RpcException)
            {
                return false;
            }
        }
    }
}
