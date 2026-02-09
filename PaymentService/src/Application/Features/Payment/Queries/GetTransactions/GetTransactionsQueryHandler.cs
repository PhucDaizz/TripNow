using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payment;
using PaymentService.Domain.Common;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.Payment.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<PagedResult<PaymentTransactionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTransactionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<PaymentTransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken token)
        {
            var currentUserId = _currentUserService.UserId;
            var role = _currentUserService.Role;

            Guid? filterUserId = null;
            if (role != AppRoles.SysAdmin && !string.IsNullOrEmpty(currentUserId))
            {
                filterUserId = Guid.Parse(currentUserId);
            }

            var pagedData = await _unitOfWork.PaymentTransactions.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                filterUserId, 
                request.BookingId,
                request.Status,
                request.Type,
                request.FromDate,
                request.ToDate,
                token
            );

            var dtos = pagedData.Items.Select(x => new PaymentTransactionDto
            {
                Id = x.Id,
                PayerUserId = x.PayerUserId,
                BookingId = x.BookingId,
                Amount = x.Amount,
                Provider = x.Provider.ToString(),
                Type = x.Type.ToString(),
                TransactionStatus = x.TransactionStatus.ToString(),
                MerchantRef = x.MerchantRef,
                ProviderTxnId = x.ProviderTxnId,
                PaymentDate = x.PaymentDate,
                FailureReason = x.FailureReason,
                Note = x.Note
            }).ToList();

            return Result.Success(new PagedResult<PaymentTransactionDto>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
