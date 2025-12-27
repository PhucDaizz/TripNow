using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Floor;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Queries.GetFloorsByBlock
{
    public class GetFloorsByBlockQuery: IRequest<Result<List<FloorDto>>>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
    }
}
