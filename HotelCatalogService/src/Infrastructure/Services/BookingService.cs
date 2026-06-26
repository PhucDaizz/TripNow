using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Infrastructure.Protos;
using Microsoft.Extensions.Logging;

namespace HotelCatalogService.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingGrpc.BookingGrpcClient _grpcClient;
        private readonly ILogger<BookingService> _logger;

        public BookingService(BookingGrpc.BookingGrpcClient grpcClient, ILogger<BookingService> logger)
        {
            _grpcClient = grpcClient;
            _logger = logger;
        }

        public async Task<bool> CheckIsHaveAnyBookInFunitue(Guid roomTypeId, CancellationToken token = default)
        {
            try
            {
                var request = new CheckRoomUsageRequest { RoomTypeId = roomTypeId.ToString() };
                var response = await _grpcClient.CheckIsHaveAnyBookInFurnitureAsync(request, cancellationToken: token);

                return response.IsUsed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi gRPC sang Booking Service");
                return false;
            }

        }
    }
}
