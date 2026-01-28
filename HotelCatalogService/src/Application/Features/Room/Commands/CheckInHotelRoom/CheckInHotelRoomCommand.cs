using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.CheckInHotelRoom
{
    public class CheckInHotelRoomCommand: IRequest<Result<RoomResponse>>
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public Guid CheckedInBy { get; set; }
        public bool IsReceptionist { get; set; }
        public Guid? UserTokenHotelId { get; set; }
    }
}
