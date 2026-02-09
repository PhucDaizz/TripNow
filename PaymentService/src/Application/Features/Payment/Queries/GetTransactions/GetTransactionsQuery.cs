using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Payment;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.Payment.Queries.GetTransactions
{
    public class GetTransactionsQuery: IRequest<Result<PagedResult<PaymentTransactionDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public Guid? BookingId { get; set; }
        public PaymentTransactionStatus? Status { get; set; }
        public TransactionType? Type { get; set; } 
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
