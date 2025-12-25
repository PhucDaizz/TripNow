using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Commands.SetMainRoomTypeImage
{
    public class SetMainRoomTypeImageCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid ImageId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
