using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.DeleteFloor
{
    public class DeleteFloorCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
