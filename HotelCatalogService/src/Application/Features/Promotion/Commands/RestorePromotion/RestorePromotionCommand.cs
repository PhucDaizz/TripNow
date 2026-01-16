using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.RestorePromotion
{
    public class RestorePromotionCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; }
    }
}
