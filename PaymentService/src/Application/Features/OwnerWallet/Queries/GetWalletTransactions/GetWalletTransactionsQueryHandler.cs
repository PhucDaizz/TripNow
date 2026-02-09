using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Wallet;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.OwnerWallet.Queries.GetWalletTransactions
{
    public class GetWalletTransactionsQueryHandler : IRequestHandler<GetWalletTransactionsQuery, Result<PagedResult<WalletLedgerDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetWalletTransactionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<WalletLedgerDto>>> Handle(GetWalletTransactionsQuery request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var pagedData = await _unitOfWork.OwnerWallets.GetPagedListAsync(
                ownerId,
                request.PageNumber,
                request.PageSize,
                request.FromDate,
                request.ToDate,
                request.Type,
                token
            );

            var dtos = pagedData.Items.Select(x => new WalletLedgerDto
            {
                Id = x.Id,
                Direction = x.Direction.ToString(), 
                Amount = x.Amount,
                BalanceAfter = x.BalanceAfter,
                Type = x.ReferenceType.ToString(),
                Description = x.Description,
                ReferenceId = x.ReferenceId,
                CreatedAt = x.CreatedAt
            }).ToList();

            return Result.Success(new PagedResult<WalletLedgerDto>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
