using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.ApplyPromotion
{
    public class ApplyPromotionCommand : IRequest<Result<decimal>>
    {
        public Guid HotelId { get; set; }
        public string Code { get; set; }
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
