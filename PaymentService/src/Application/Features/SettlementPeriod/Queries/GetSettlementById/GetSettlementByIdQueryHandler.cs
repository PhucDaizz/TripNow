using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Settlement;
using PaymentService.Application.DTOs.SettlementItem;

namespace PaymentService.Application.Features.SettlementPeriod.Queries.GetSettlementById
{
    public class GetSettlementByIdQueryHandler : IRequestHandler<GetSettlementByIdQuery, Result<SettlementDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSettlementByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<SettlementDetailDto>> Handle(GetSettlementByIdQuery request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var settlement = await _unitOfWork.SettlementPeriods.GetByIdWithItemsAsync(request.Id);

            if (settlement == null)
                return Result.Failure<SettlementDetailDto>(new Error("Settlement.NotFound", "This Settlement was not found"));

            if (settlement.OwnerId != ownerId)
                return Result.Failure<SettlementDetailDto>(new Error("Auth.Forbidden", "You do not have permission to view this reconciliation period."));

            // Map Data
            var dto = new SettlementDetailDto
            {
                Id = settlement.Id,
                PeriodFrom = settlement.PeriodFrom,
                PeriodTo = settlement.PeriodTo,
                Status = settlement.Status.ToString(),
                TotalGross = settlement.TotalGross,
                TotalCommission = settlement.TotalCommission,
                TotalNetPayable = settlement.TotalNetPayable,
                CreatedAt = settlement.CreatedAt,

                // Map Items
                Items = settlement.SettlementItems.Select(i => new SettlementItemDto
                {
                    BookingId = i.BookingId,
                    GrossAmount = i.GrossAmount,
                    CommissionAmount = i.CommissionAmount,
                    NetAmount = i.NetAmount,
                    Type = i.Type.ToString()
                }).ToList()
            };

            return Result.Success(dto);
        }
    }
}
