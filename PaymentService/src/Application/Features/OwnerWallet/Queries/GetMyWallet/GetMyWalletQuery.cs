using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Wallet;

namespace PaymentService.Application.Features.OwnerWallet.Queries.GetMyWallet
{
    public class GetMyWalletQuery : IRequest<Result<OwnerWalletSummaryDto>> { }
}
