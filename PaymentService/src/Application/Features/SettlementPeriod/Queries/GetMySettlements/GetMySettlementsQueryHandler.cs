using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Settlement;
using PaymentService.Domain.Common;
using PaymentService.Domain.Common.Models;

namespace PaymentService.Application.Features.SettlementPeriod.Queries.GetMySettlements
{
    public class GetMySettlementsQueryHandler : IRequestHandler<GetMySettlementsQuery, Result<PagedResult<SettlementPeriodDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMySettlementsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<SettlementPeriodDto>>> Handle(GetMySettlementsQuery request, CancellationToken token)
        {
            Guid? ownerId = Guid.Parse(_currentUserService.UserId);

            var role = _currentUserService.Role;

            if (role == AppRoles.SysAdmin)
            {
                ownerId = null; 
            }

            var pagedData = await _unitOfWork.SettlementPeriods.GetPagedListAsync(
                ownerId,
                request.PageNumber,
                request.PageSize,
                request.FromDate,
                request.ToDate,
                token
            );

            var dtos = pagedData.Items.Select(x => new SettlementPeriodDto
            {
                Id = x.Id,
                PeriodFrom = x.PeriodFrom,
                PeriodTo = x.PeriodTo,
                Status = x.Status.ToString(),
                TotalGross = x.TotalGross,
                TotalCommission = x.TotalCommission,
                TotalNetPayable = x.TotalNetPayable,
                CreatedAt = x.CreatedAt
            }).ToList();

            return Result.Success(new PagedResult<SettlementPeriodDto>(dtos, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize));
        }
    }
}
