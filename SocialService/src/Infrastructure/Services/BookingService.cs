using SocialService.Infrastructure.Protos;
using Grpc.Core;
using SocialService.Application.Contracts;

namespace SocialService.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingGrpc.BookingGrpcClient _grpcClient;

        public BookingService(BookingGrpc.BookingGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<bool> IsBookingExisting(Guid bookingId, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new IsBookingExistingRequest
                {
                    BookingId = bookingId.ToString()
                };

                var response = await _grpcClient.IsBookingExistingAsync(request, cancellationToken: cancellationToken);

                return response.IsExisting;
            }
            catch (RpcException)
            {
                return false;
            }
        }
    }
}
