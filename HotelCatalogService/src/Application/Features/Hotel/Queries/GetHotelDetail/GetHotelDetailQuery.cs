using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetail
{
    public class GetHotelDetailQuery: IRequest<Result<HotelDetailDto>>
    {
        public Guid HotelId { get; set; }
    }
}
