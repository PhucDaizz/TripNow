using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.AddHotelStructure
{
    public record CreateBlockDto
    {
        public string BlockName { get; init; } 
        public List<CreateFloorDto> Floors { get; init; }
    }

    public record CreateFloorDto
    {
        public int FloorName { get; init; } 
        public List<CreateRoomDto> Rooms { get; init; }
    }

    public record CreateRoomDto
    {
        public Guid RoomTypeId { get; init; }
        public string RoomName { get; init; } 
    }

    public record AddHotelStructureCommand : IRequest<Result>
    {
        public Guid HotelId { get; init; }
        public Guid OwnerId { get; set; }
        public List<CreateBlockDto> Blocks { get; init; }
    }
}
