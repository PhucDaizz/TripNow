using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.Payout.Commands.RejectPayout
{
    public class RejectPayoutCommand: IRequest<Result>
    {
        public Guid PayoutId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
    }
}
