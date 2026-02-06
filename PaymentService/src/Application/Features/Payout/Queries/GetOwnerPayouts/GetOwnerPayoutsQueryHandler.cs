using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payout;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.Payout.Queries.GetOwnerPayouts
{
    public class GetOwnerPayoutsQueryHandler : IRequestHandler<GetOwnerPayoutsQuery, Result<PagedResult<PayoutDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService; 

        public GetOwnerPayoutsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<PayoutDTO>>> Handle(GetOwnerPayoutsQuery request, CancellationToken token)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            if (currentUserId == Guid.Empty)
            {
                return Result.Failure<PagedResult<PayoutDTO>>(new Error("Auth.UnAuthorized", "Người dùng chưa đăng nhập."));
            }

            var wallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(currentUserId, token);

            if (wallet == null)
            {
                return Result.Success(PagedResult<PayoutDTO>.Empty());
            }

            var pagedData = await _unitOfWork.Payouts.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                wallet.Id, 
                null,      
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

            var result = new PagedResult<PayoutDTO>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize);

            return Result.Success(result);
        }
    }
}
