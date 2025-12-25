using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.RoomType;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetRoomTypesByHotel
{
    public class GetRoomTypesByHotelQuery: IRequest<Result<List<RoomTypeDto>>>
    {
        public Guid HotelId { get; init; }
    }
}
