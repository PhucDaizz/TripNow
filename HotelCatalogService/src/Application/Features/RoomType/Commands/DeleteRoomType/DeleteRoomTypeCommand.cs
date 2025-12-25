using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomType.Commands.DeleteRoomType
{
    public class DeleteRoomTypeCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid OwnerId { get; set; } 
    }
}
