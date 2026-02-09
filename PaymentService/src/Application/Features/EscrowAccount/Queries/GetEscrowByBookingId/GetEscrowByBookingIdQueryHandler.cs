using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.EscrowAccount;

namespace PaymentService.Application.Features.EscrowAccount.Queries.GetEscrowByBookingId
{
    public class GetEscrowByBookingIdQueryHandler : IRequestHandler<GetEscrowByBookingIdQuery, Result<EscrowAccountDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEscrowByBookingIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EscrowAccountDto>> Handle(GetEscrowByBookingIdQuery request, CancellationToken token)
        {
            var escrow = await _unitOfWork.EscrowAccounts.GetByBookingIdAsync(request.BookingId);

            if (escrow == null)
                return Result.Failure<EscrowAccountDto>(new Error("Escrow.NotFound", "Could not find the escrow wallet for this order"));

            var actualRevenue = escrow.Amount - escrow.RefundedAmount;
            var netToOwner = actualRevenue - escrow.ProviderFee;

            var dto = new EscrowAccountDto
            {
                Id = escrow.Id,
                BookingId = escrow.BookingId,
                Amount = escrow.Amount,
                RefundedAmount = escrow.RefundedAmount,
                ProviderFee = escrow.ProviderFee,
                Status = escrow.Status.ToString(),
                ActualRevenue = actualRevenue,
                NetToOwner = netToOwner > 0 ? netToOwner : 0,
                CreatedAt = escrow.CreatedAt,
                LastModified = escrow.UpdatedAt
            };

            return Result.Success(dto);
        }
    }
}
