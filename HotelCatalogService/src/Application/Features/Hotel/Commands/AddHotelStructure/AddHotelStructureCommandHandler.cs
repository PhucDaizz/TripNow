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
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Blocks);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Hotel not found!"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Not permitted"));

            var existingBlockNames = hotel.Blocks
                .Select(b => b.Name.ToLower())
                .ToHashSet();

            foreach (var blockDto in request.Blocks)
            {
                if (existingBlockNames.Contains(blockDto.BlockName.ToLower()))
                {
                    return Result.Failure(new Error(
                        "Hotel.BlockExists",
                        $"The '{blockDto.BlockName}' field already exists. Please use the manual add room function.."
                    ));
                }
            }


            var inventoryChanges = new Dictionary<Guid, int>();

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
            }

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
