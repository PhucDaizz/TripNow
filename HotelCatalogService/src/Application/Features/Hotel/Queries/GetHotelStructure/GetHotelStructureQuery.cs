using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelStructure
{
    public record RoomResponseDto(Guid Id, string RoomName, Guid RoomTypeId);

    public record FloorResponseDto(Guid Id, int FloorName, List<RoomResponseDto> Rooms);

    public record BlockResponseDto(Guid Id, string BlockName, List<FloorResponseDto> Floors);

    public record GetHotelStructureQuery(Guid HotelId) : IRequest<Result<List<BlockResponseDto>>>;
}
