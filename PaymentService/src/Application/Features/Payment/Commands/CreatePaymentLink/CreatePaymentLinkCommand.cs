using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Payment;

namespace PaymentService.Application.Features.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkCommand: IRequest<Result<string>>
    {
        public ProviderBank providerBank { get; set; }
        public string BookingId { get; set; }
        public double MoneyToPay { get; set; }
    }
}
