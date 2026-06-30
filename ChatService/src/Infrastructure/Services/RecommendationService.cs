using ChatService.Application.Common.Interfaces;
using ChatService.Infrastructure.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace ChatService.Infrastructure.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly RagGrpc.RagGrpcClient _grpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecommendationService(RagGrpc.RagGrpcClient grpcClient, IHttpContextAccessor httpContextAccessor)
        {
            _grpcClient = grpcClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<string>> GetHotelChatContextAsync(Guid hotelId, string userMessage, int limit = 3, CancellationToken cancellationToken = default)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                var bearerToken = context?.Request.Headers["Authorization"].ToString();

                var headers = new Metadata();
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    headers.Add("Authorization", bearerToken);
                }

                var request = new GetHotelChatContextRequest
                {
                    HotelId = hotelId.ToString(),
                    Message = userMessage,
                    Limit = limit
                };

                var response = await _grpcClient.GetHotelChatContextAsync(
                    request,
                    headers: headers,
                    cancellationToken: cancellationToken);

                return response.Contexts.ToList();
            }
            catch (RpcException)
            {
                return new List<string>();
            }
        }
    }
}
