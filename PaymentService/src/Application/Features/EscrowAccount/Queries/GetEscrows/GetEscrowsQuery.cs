using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.EscrowAccount;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.EscrowAccount.Queries.GetEscrows
{
    public class GetEscrowsQuery : IRequest<Result<PagedResult<EscrowAccountDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public EscrowStatus? Status { get; set; }    // Lọc: Chỉ lấy đơn đang Holding
        public Guid? BookingId { get; set; }         // Tìm đích danh
        public DateTime? FromDate { get; set; }      // Từ ngày
        public DateTime? ToDate { get; set; }        // Đến ngày
    }
}
