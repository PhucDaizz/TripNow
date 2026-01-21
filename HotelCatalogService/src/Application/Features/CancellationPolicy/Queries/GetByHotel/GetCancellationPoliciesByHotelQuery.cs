using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.CancellationPolicy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Queries.GetByHotel
{
    public class GetCancellationPoliciesByHotelQuery : IRequest<Result<List<CancellationPolicyDto>>>
    {
        public Guid HotelId { get; set; }
    }

    public class GetCancellationPoliciesByHotelQueryHandler : IRequestHandler<GetCancellationPoliciesByHotelQuery, Result<List<CancellationPolicyDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCancellationPoliciesByHotelQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<CancellationPolicyDto>>> Handle(GetCancellationPoliciesByHotelQuery request, CancellationToken cancellationToken)
        {
            var policies = await _context.CancellationPolicy
                .Where(p => p.HotelId == request.HotelId)
                .Select(p => new CancellationPolicyDto
                {
                    Id = p.Id,
                    HotelId = p.HotelId,
                    Name = p.Name,
                    Type = p.Type.ToString(),
                    Description = p.Description,
                    Rules = p.Rules.Select(r => new DTOs.CancellationPolicy.CancellationRuleDto
                    {
                        Id = r.Id,
                        HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                        RefundPercentage = r.RefundPercentage
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return Result<List<CancellationPolicyDto>>.Success(policies);
        }
    }
}
