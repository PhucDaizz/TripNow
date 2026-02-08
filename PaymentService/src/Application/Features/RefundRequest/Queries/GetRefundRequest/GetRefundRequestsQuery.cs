using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.RefundRequest;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.RefundRequest.Queries.GetRefundRequest
{
    public class GetRefundRequestsQuery : IRequest<Result<PagedResult<RefundRequestDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public RefundStatus? Status { get; set; } // Null = Lấy hết, 0 = Pending
        public string? SearchBookingId { get; set; }     // Tìm theo mã đơn
    }
}
