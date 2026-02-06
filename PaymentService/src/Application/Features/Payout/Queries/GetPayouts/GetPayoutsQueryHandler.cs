using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payout;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.Payout.Queries.GetPayouts
{
    public class GetPayoutsQueryHandler : IRequestHandler<GetPayoutsQuery, Result<PagedResult<PayoutDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPayoutsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<PayoutDTO>>> Handle(GetPayoutsQuery request, CancellationToken token)
        {
            var pagedData = await _unitOfWork.Payouts.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.OwnerWalletId,
                request.SearchTransactionRef,
                request.FromDate,
                request.ToDate,
                token
            );

            var dtos = pagedData.Items.Select(x => new PayoutDTO
            {
                Id = x.Id,
                SettlementId = x.SettlementId,
                OwnerWalletId = x.OwnerWalletId,
                BankInfo = x.BankInfo, 
                Amount = x.Amount,
                TransactionReference = x.TransactionReference,
                FailureReason = x.FailureReason,
                Status = x.Status,
                CreatedAt = x.CreatedAt 
            }).ToList();

            var result = new PagedResult<PayoutDTO>(
                dtos,
                pagedData.TotalCount,
                pagedData.PageNumber,
                pagedData.PageSize
            );

            return Result.Success(result);
        }
    }
}
