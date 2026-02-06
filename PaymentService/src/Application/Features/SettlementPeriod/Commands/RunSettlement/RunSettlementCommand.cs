using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.SettlementPeriod.Commands.RunSettlement
{
    public class RunSettlementCommand : IRequest<Result>
    {
        // public DateTime? ManualDate { get; set; }
    }
}
