using Domain.Common.Response;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.UpdatePromotion
{
    public class UpdatePromotionCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid PromotionId { get; set; }
        public Guid OwnerId { get; set; }
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
    }
}
