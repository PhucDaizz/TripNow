using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.CancellationPolicy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Queries.GetById
{
    public class GetCancellationPolicyByIdQuery : IRequest<Result<CancellationPolicyDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetCancellationPolicyByIdQueryHandler : IRequestHandler<GetCancellationPolicyByIdQuery, Result<CancellationPolicyDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCancellationPolicyByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CancellationPolicyDto>> Handle(GetCancellationPolicyByIdQuery request, CancellationToken cancellationToken)
        {
            var policy = await _context.CancellationPolicy
                .Where(p => p.Id == request.Id)
                .Select(p => new CancellationPolicyDto
                {
                    Id = p.Id,
                    HotelId = p.HotelId,
                    Name = p.Name,
                    Type = p.Type.ToString(),
                    Description = p.Description,
                    Rules = p.Rules.Select(r => new CancellationRuleDto
                    {
                        Id = r.Id,
                        HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                        RefundPercentage = r.RefundPercentage
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (policy == null)
            {
                return Result.Failure<CancellationPolicyDto>(new Error("NOT.FOUND", "CancellationPolicy not found."));
            }

            return Result<CancellationPolicyDto>.Success(policy);
        }
    }
}
