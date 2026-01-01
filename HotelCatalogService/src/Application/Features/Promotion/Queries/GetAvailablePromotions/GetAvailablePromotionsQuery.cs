using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Promotion;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Queries.GetAvailablePromotions
{
    public class GetAvailablePromotionsQuery : IRequest<Result<List<UserPromotionDto>>>
    {
        public Guid HotelId { get; set; }
    }
}
