using BookingService.Application.Contracts;
using BookingService.Infrastructure.Protos;
using Grpc.Core;

namespace BookingService.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentGrpc.PaymentGrpcClient _grpcClient;

        public PaymentService(PaymentGrpc.PaymentGrpcClient  grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, Application.DTOs.Payment.PaymentProvider paymentProvider, CancellationToken token)
        {
            try
            {
                var request = new CreatePaymentLinkRequest
                {
                    BookingId = bookingId.ToString(),
                    Amount = (double)amount,
                    PaymentProvider = (PaymentProvider)paymentProvider
                };

                var response = await _grpcClient.CreatePaymentLinkAsync(request, cancellationToken: token);

                return response.PaymentLink;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to create payment link. gRPC Status: {ex.StatusCode}, Detail: {ex.Status.Detail}");
            }
        }
    }
}
