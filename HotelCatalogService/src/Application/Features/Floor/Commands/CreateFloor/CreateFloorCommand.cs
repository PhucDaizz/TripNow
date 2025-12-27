using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.CreateFloor
{
    public class CreateFloorCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid OwnerId { get; set; }
        public int FloorNumber { get; set; }
    }
}
