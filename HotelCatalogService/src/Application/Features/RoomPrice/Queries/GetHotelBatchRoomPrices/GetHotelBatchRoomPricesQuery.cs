using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetHotelBatchRoomPrices
{
    public class GetHotelBatchRoomPricesQuery: IRequest<Result<List<RoomTypeCalendarDto>>>
    {
        public Guid HotelId { get; set; } 
        // Hoặc: public List<Guid> RoomTypeIds { get; set; } // Nếu muốn lấy theo list chỉ định
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
