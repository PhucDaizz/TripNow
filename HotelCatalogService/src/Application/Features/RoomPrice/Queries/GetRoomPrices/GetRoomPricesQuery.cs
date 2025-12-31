using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomPrices
{
    public class GetRoomPricesQuery : IRequest<Result<List<RoomPriceDto>>>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
