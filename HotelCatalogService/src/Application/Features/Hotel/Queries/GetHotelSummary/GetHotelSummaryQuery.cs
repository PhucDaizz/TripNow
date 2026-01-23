using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelSummary
{
    public class GetHotelSummaryQuery: IRequest<Result<HotelSummaryDto>>
    {
        public Guid HotelId { get; set; }
    }
}
