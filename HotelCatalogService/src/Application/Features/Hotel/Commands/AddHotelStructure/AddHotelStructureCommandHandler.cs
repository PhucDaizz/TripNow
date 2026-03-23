using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.AddHotelStructure
{
    public class AddHotelStructureCommandHandler : IRequestHandler<AddHotelStructureCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public AddHotelStructureCommandHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result> Handle(AddHotelStructureCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithFullStructureAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Hotel not found!"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Not permitted"));


            var trackedBlockNames = hotel.Blocks.Select(b => b.Name.ToLower()).ToHashSet();
            var trackedRoomNames = hotel.Blocks
                .SelectMany(b => b.Floors)
                .SelectMany(f => f.Rooms)
                .Select(r => r.RoomName.Trim().ToLower())
                .ToHashSet();

            var inventoryChanges = new Dictionary<Guid, int>();

            foreach (var blockDto in request.Blocks)
            {
                var normalizedBlockName = blockDto.BlockName.Trim().ToLower();

                if (!trackedBlockNames.Add(normalizedBlockName)) 
                {
                    return Result.Failure(new Error("Hotel.BlockExists", $"Tòa nhà '{blockDto.BlockName}' bị trùng lặp."));
                }

                var block = hotel.AddBlock(blockDto.BlockName);
                var trackedFloorNames = new HashSet<int>();

                foreach (var floorDto in blockDto.Floors)
                {
                    if (!trackedFloorNames.Add(floorDto.FloorName))
                    {
                        return Result.Failure(new Error(
                            "Hotel.FloorExists",
                            $"Tầng '{floorDto.FloorName}' bị trùng lặp trong tòa {blockDto.BlockName}."));
                    }

                    var floor = block.AddFloor(floorDto.FloorName);

                    foreach (var roomDto in floorDto.Rooms)
                    {
                        var normalizedRoomName = roomDto.RoomName.Trim().ToLower();

                        if (!trackedRoomNames.Add(normalizedRoomName))
                        {
                            return Result.Failure(new Error("Hotel.RoomExists", $"Tên phòng '{roomDto.RoomName}' bị trùng lặp trong khách sạn."));
                        }

                        floor.AddRoomNoEvent(roomDto.RoomName, roomDto.RoomTypeId);

                        if (!inventoryChanges.ContainsKey(roomDto.RoomTypeId))
                        {
                            inventoryChanges[roomDto.RoomTypeId] = 0;
                        }
                        inventoryChanges[roomDto.RoomTypeId]++;
                    }
                }
            }

            /*var inventoryChanges = new Dictionary<Guid, int>();

            foreach (var blockDto in request.Blocks)
            {
                var block = hotel.AddBlock(blockDto.BlockName);

                foreach (var floorDto in blockDto.Floors)
                {
                    var floor = block.AddFloor(floorDto.FloorName);

                    foreach (var roomDto in floorDto.Rooms)
                    {
                        floor.AddRoomNoEvent(roomDto.RoomName, roomDto.RoomTypeId);

                        if (!inventoryChanges.ContainsKey(roomDto.RoomTypeId))
                        {
                            inventoryChanges[roomDto.RoomTypeId] = 0;
                        }
                        inventoryChanges[roomDto.RoomTypeId]++;
                    }
                }
            }*/

            await _unitOfWork.SaveChangesAsync(token);

            foreach (var change in inventoryChanges)
            {
                await _integrationEventService.PublishAsync<InventoryStockChangedEvent>(new InventoryStockChangedEvent
                {
                    RoomTypeId = change.Key,
                    QuantityChange = change.Value
                }, "hotel-catalog.events", "topic", "room.range.created", token);
            }

            return Result.Success();
        }
    }
}
