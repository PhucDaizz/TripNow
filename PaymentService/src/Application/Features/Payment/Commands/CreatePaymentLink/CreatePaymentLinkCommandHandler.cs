using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment;

namespace PaymentService.Application.Features.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkCommandHandler : IRequestHandler<CreatePaymentLinkCommand, Result<string>>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentLinkCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<Result<string>> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var description = $"Payment for booking {request.BookingId}";
            var paymentLink = string.Empty;
            switch (request.providerBank)
            {
                case ProviderBank.VNPAY:
                    paymentLink = await _paymentService.CreateVNPaymentLink(request.MoneyToPay, description);
                    break;
                default:
                    paymentLink = await _paymentService.CreateVNPaymentLink(request.MoneyToPay, description);
                    break;
            }
            return Result.Success<string>(paymentLink);
        }
    }
}
