using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Promotion;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Promotion.Queries.CheckPromotion
{
    public class CheckPromotionQueryHandler : IRequestHandler<CheckPromotionQuery, Result<PromotionDiscountDto>>
    {
        private readonly IApplicationDbContext _context;

        public CheckPromotionQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PromotionDiscountDto>> Handle(CheckPromotionQuery request, CancellationToken token)
        {
            var now = DateTime.UtcNow;

            var promo = await _context.Promotion.AsNoTracking()
                .FirstOrDefaultAsync(p => p.HotelId == request.HotelId && p.Code == request.Code, token);

            if (promo == null) return Result.Failure<PromotionDiscountDto>(new Error("Promotion.NotFound", "The discount code does not exist.."));

            if (!promo.IsActive) return Result.Failure<PromotionDiscountDto>(new Error("Promotion.Inactive", "The discount code has been deactivated."));
            if (now < promo.StartDate || now > promo.EndDate) return Result.Failure<PromotionDiscountDto>(new Error("Promotion.Expired", "The discount code has either not started or has expired."));
            if (promo.RemainingQuantity <= 0) return Result.Failure<PromotionDiscountDto>(new Error("Promotion.OutOfStock", "The discount code has expired."));

            var hasUsed = await _context.PromotionUsage
                .AnyAsync(u => u.PromotionId == promo.Id && u.UserId == request.UserId, token);

            if (hasUsed)
            {
                return Result.Failure<PromotionDiscountDto>(new Error("Promotion.Used", "You have already used this discount code."));
            }

            if (request.OrderAmount < promo.MinBookingAmount)
            {
                decimal missingAmount = promo.MinBookingAmount - request.OrderAmount;
                return Result.Failure<PromotionDiscountDto>(
                    new Error("Promotion.NotEligible",
                    $"Your order is not eligible. You need to add {missingAmount:N0}đ to use this code.")
                );
            }

            decimal finalAmount = 0;
            if (promo.DiscountType == DiscountType.Amount)
            {
                finalAmount = promo.DiscountValue;
            }
            else 
            {
                finalAmount = request.OrderAmount * (promo.DiscountValue / 100);
            }

            if (finalAmount > request.OrderAmount) finalAmount = request.OrderAmount;

            return Result.Success(new PromotionDiscountDto
            {
                PromotionId = promo.Id,
                Code = promo.Code,
                DiscountType = promo.DiscountType.ToString(),
                DiscountValue = promo.DiscountValue,
                FinalDiscountAmount = finalAmount
            });
        }
    }
}
