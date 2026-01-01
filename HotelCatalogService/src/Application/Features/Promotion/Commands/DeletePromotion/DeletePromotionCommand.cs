using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.DeletePromotion
{
    public class DeletePromotionCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid PromotionId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
