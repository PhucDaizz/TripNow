using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.RefundRequest;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.RefundRequest.Queries.GetRefundRequest
{
    public class GetRefundRequestsQueryHandler : IRequestHandler<GetRefundRequestsQuery, Result<PagedResult<RefundRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRefundRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<RefundRequestDto>>> Handle(GetRefundRequestsQuery request, CancellationToken token)
        {
            var pagedData = await _unitOfWork.RefundRequests.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.SearchBookingId,
                token
            );

            var dtos = pagedData.Items.Select(x => new RefundRequestDto
            {
                Id = x.Id,
                BookingId = x.BookingId,
                UserId = x.UserId,
                Amount = x.Amount,
                OriginalGatewayTransactionRef = x.OriginalGatewayTransactionRef,
                RefundGatewayTransactionRef = x.RefundGatewayTransactionRef,
                Status = x.Status,
                Reason = x.Reason,
                AdminNote = x.AdminNote,
                CreatedAt = x.CreatedAt,
                ProcessedAt = x.ProcessedAt
            }).ToList();

            return Result.Success(new PagedResult<RefundRequestDto>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
