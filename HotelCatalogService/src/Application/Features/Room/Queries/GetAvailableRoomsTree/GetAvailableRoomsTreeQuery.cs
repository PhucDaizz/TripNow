using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Block;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Queries.GetAvailableRoomsTree
{
    public class GetAvailableRoomsTreeQuery: IRequest<Result<List<BlockResponse>>>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
