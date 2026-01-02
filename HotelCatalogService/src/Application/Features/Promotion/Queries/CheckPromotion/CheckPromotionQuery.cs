using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Promotion;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Queries.CheckPromotion
{
    public class CheckPromotionQuery : IRequest<Result<PromotionDiscountDto>>
    {
        public Guid HotelId { get; set; }
        public string Code { get; set; }
        public Guid UserId { get; set; } // Quan trọng: Check xem user này xài chưa
        public decimal OrderAmount { get; set; } // Giá trị đơn hàng (để tính số tiền giảm nếu là %)
    }
}
