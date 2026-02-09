using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Payout;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.Payout.Queries.GetAdminPayouts
{
    public class GetAdminPayoutsQuery : IRequest<Result<PagedResult<PayoutDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Filter cho Admin
        public Guid? OwnerWalletId { get; set; }
        public PayoutStatus? Status { get; set; } 
        public string? TransactionRef { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
