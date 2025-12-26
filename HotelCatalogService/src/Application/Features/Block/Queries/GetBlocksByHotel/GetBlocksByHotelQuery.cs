using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Block;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Queries.GetBlocksByHotel
{
    public class GetBlocksByHotelQuery: IRequest<Result<List<BlockDto>>>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
