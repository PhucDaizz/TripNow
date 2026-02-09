using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Settlement;

namespace PaymentService.Application.Features.SettlementPeriod.Queries.GetSettlementById
{
    public record GetSettlementByIdQuery(Guid Id) : IRequest<Result<SettlementDetailDto>>;
}
