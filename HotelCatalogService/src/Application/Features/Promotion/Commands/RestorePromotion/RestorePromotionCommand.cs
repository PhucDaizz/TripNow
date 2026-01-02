using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.RestorePromotion
{
    public class RestorePromotionCommand : IRequest<Result>
    {
        public Guid BookingId { get; set; }
    }
}
