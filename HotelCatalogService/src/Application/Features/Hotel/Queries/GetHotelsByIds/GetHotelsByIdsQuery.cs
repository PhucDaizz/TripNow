using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsByIds
{
    public record GetHotelsByIdsQuery(List<Guid> HotelIds) : IRequest<Result<IEnumerable<HotelSummaryDto>>>;
}
