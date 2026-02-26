using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetailBySlug
{
    public class GetHotelDetailBySlugQuery : IRequest<Result<HotelDetailDto>>
    {
        public string Slug { get; set; } = string.Empty;
    }
}
