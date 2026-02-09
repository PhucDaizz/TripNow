using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Wallet;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.OwnerWallet.Queries.GetWalletTransactions
{
    public class GetWalletTransactionsQuery : IRequest<Result<PagedResult<WalletLedgerDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Type { get; set; } 
    }
}
