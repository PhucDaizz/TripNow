using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Promotion;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Promotion.Queries.GetAvailablePromotions
{
    public class GetAvailablePromotionsQueryHandler : IRequestHandler<GetAvailablePromotionsQuery, Result<List<UserPromotionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAvailablePromotionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<UserPromotionDto>>> Handle(GetAvailablePromotionsQuery request, CancellationToken token)
        {
            var now = DateTime.UtcNow;

            var entities = await _context.Promotion.AsNoTracking()
                .Where(p => p.HotelId == request.HotelId)
                .Where(p => p.IsActive == true)              
                .Where(p => p.RemainingQuantity > 0)         
                .Where(p => p.StartDate <= now && p.EndDate >= now) 
                .OrderBy(p => p.EndDate) 
                .ToListAsync(token);

            var dtos = entities.Select(p =>
            {
                string label = "Ưu đãi";
                if (p.RemainingQuantity <= 10) label = "Sắp hết";
                else if ((p.EndDate - now).TotalHours < 24) label = "Gấp";
                else if (p.DiscountType == DiscountType.Percent && p.DiscountValue >= 50) label = "Siêu Hot";

                return new UserPromotionDto
                {
                    Id = p.Id,
                    Code = p.Code,

                    DiscountInfo = p.DiscountType == DiscountType.Percent
                        ? $"Giảm {p.DiscountValue:0.#}%"
                        : $"Giảm {p.DiscountValue:N0}đ",

                    EndDate = p.EndDate,

                    RemainingCount = p.RemainingQuantity <= 20 ? p.RemainingQuantity : null,

                    TagLabel = label
                };
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
