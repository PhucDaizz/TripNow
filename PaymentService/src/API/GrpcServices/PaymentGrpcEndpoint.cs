using Grpc.Core;
using MediatR;
using PaymentService.Api.Protos;
using PaymentService.Application.Features.Payment.Commands.CreatePaymentLink;

namespace PaymentService.API.GrpcServices
{
    public class PaymentGrpcEndpoint : PaymentGrpc.PaymentGrpcBase
    {
        private readonly IMediator _mediator;

        public PaymentGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CreatePaymentLinkResponse> CreatePaymentLink(
            CreatePaymentLinkRequest request,
            ServerCallContext context)
        {
            var command = new CreatePaymentLinkCommand
            {
                providerBank = (Domain.Enum.PaymentProvider)request.PaymentProvider,
                BookingId = request.BookingId, 
                MoneyToPay = request.Amount    
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return new CreatePaymentLinkResponse
                {
                    PaymentLink = result.Value
                };
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Message ?? "Failed to create payment link."));
        }
    }
}
