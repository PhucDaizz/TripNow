using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payout;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.Payout.Queries.GetAdminPayouts
{
    public class GetAdminPayoutsQueryHandler : IRequestHandler<GetAdminPayoutsQuery, Result<PagedResult<PayoutDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminPayoutsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<PayoutDTO>>> Handle(GetAdminPayoutsQuery request, CancellationToken token)
        {
            var pagedData = await _unitOfWork.Payouts.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.OwnerWalletId,
                request.TransactionRef,
                request.FromDate,
                request.ToDate,
                token
            );

            var dtos = pagedData.Items.Select(p => new PayoutDTO
            {
                Id = p.Id,
                SettlementId = p.SettlementId,
                OwnerWalletId = p.OwnerWalletId,
                BankInfo = p.BankInfo,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                TransactionReference = p.TransactionReference,
                FailureReason = p.FailureReason,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Result.Success(new PagedResult<PayoutDTO>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
