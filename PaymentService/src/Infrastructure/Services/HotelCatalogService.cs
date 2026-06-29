using Grpc.Core;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.HotelCatalog;
using PaymentService.Infrastructure.Protos;

namespace PaymentService.Infrastructure.Services
{
    public class HotelCatalogService : IHotelCatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _grpcClient;

        public HotelCatalogService(CatalogGrpc.CatalogGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default)
        {
            try
            {
                var request = new GetHotelSummaryRequest
                {
                    HotelId = hotelId.ToString()
                };

                var response = await _grpcClient.GetHotelSummaryAsync(request, cancellationToken: token);

                return new HotelSummaryDto
                {
                    HotelName = response.HotelName,
                    OwnerId = Guid.Parse(response.OwnerId), 
                    Street = response.Street,
                    City = response.City,
                    Country = response.Country,
                    Status = response.Status
                };
            }
            catch (RpcException)
            {
                return null;
            }
        }
    }
}
