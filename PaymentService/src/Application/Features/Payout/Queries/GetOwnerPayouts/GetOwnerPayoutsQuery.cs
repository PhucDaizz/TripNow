using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Payout;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.Payout.Queries.GetOwnerPayouts
{
    public class GetOwnerPayoutsQuery : IRequest<Result<PagedResult<PayoutDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PayoutStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
