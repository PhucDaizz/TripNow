using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.SettlementPeriod.Commands.RetrySettlement
{
    public class RetrySettlementCommand : IRequest<Result>
    {
        public Guid OwnerId { get; set; }
    }
}
