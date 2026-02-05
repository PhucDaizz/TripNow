using Domain.Common.Response;
using MediatR;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkCommand: IRequest<Result<string>>
    {
        public PaymentProvider providerBank { get; set; }
        public string BookingId { get; set; }
        public double MoneyToPay { get; set; }
        public Guid? PayerUserId { get; set; }
    }
}
