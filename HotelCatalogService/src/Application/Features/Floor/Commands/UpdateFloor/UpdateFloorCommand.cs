using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.UpdateFloor
{
    public class UpdateFloorCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid OwnerId { get; set; }
        public int FloorNumber { get; set; }
    }
}
