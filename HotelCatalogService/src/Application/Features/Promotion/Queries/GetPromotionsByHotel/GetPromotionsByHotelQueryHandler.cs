using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Promotion;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.Application.Features.Promotion.Queries.GetPromotionsByHotel
{
    public class GetPromotionsByHotelQueryHandler : IRequestHandler<GetPromotionsByHotelQuery, Result<PagedResult<PromotionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPromotionsByHotelQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<PromotionDto>>> Handle(GetPromotionsByHotelQuery request, CancellationToken token)
        {
            var query = _context.Promotion
                .AsNoTracking()
                .Where(p => p.HotelId == request.HotelId);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var term = request.SearchTerm.ToUpper();
                query = query.Where(p => p.Code.Contains(term));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(token);

            var entities = await query
                .OrderByDescending(p => p.StartDate) 
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(token);

            var now = DateTime.UtcNow;

            var dtos = entities.Select(p => new PromotionDto
            {
                Id = p.Id,
                Code = p.Code,
                Type = p.DiscountType.ToString(),
                Value = p.DiscountValue,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                InitialQuantity = p.InitialQuantity,
                RemainingQuantity = p.RemainingQuantity,
                IsActive = p.IsActive,

                Status = !p.IsActive ? "Deactivated" :
                         (now > p.EndDate) ? "Expired" :
                         (p.RemainingQuantity <= 0) ? "Out of Stock" :
                         (now < p.StartDate) ? "Upcoming" : "Active"
            }).ToList();

            var pagedResult = PagedResult<PromotionDto>.Create(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            return Result.Success(pagedResult);
        }
    }
}
