using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.Payout.Commands.AcceptPayout
{
    public class AcceptPayoutCommand: IRequest<Result>
    {
        public Guid PayoutId { get; set; }
        public string TransactionReference { get; set; }
    }
}
