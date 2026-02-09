using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Settlement;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.SettlementPeriod.Queries.GetMySettlements
{
    public class GetMySettlementsQuery : IRequest<Result<PagedResult<SettlementPeriodDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12; 

        // Filter
        public DateTime? FromDate { get; set; } 
        public DateTime? ToDate { get; set; }
    }
}
