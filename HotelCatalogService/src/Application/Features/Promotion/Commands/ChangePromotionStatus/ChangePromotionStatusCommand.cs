using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.ChangePromotionStatus
{
    public class ChangePromotionStatusCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid PromotionId { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsActive { get; set; }
    }
}
