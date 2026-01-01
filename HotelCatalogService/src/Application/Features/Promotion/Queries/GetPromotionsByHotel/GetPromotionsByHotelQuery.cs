using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Promotion;
using MediatR;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.Application.Features.Promotion.Queries.GetPromotionsByHotel
{
    public class GetPromotionsByHotelQuery : IRequest<Result<PagedResult<PromotionDto>>>
    {
        public Guid HotelId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? SearchTerm { get; set; } 
        public bool? IsActive { get; set; } // Lọc: null=lấy hết, true=đang bật, false=đang tắt
    }
}
