using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.EscrowAccount;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.EscrowAccount.Queries.GetEscrows
{
    public class GetEscrowsQueryHandler : IRequestHandler<GetEscrowsQuery, Result<PagedResult<EscrowAccountDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEscrowsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<EscrowAccountDto>>> Handle(GetEscrowsQuery request, CancellationToken token)
        {
            var pagedData = await _unitOfWork.EscrowAccounts.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.BookingId,
                request.FromDate,
                request.ToDate,
                token
            );

            var dtos = pagedData.Items.Select(e => {
                var actualRevenue = e.Amount - e.RefundedAmount;
                return new EscrowAccountDto
                {
                    Id = e.Id,
                    BookingId = e.BookingId,
                    Amount = e.Amount,
                    RefundedAmount = e.RefundedAmount,
                    ProviderFee = e.ProviderFee,
                    Status = e.Status.ToString(),
                    ActualRevenue = actualRevenue,
                    NetToOwner = actualRevenue - e.ProviderFee,
                    CreatedAt = e.CreatedAt,
                    LastModified = e.UpdatedAt
                };
            }).ToList();

            return Result.Success(new PagedResult<EscrowAccountDto>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
