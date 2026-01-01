using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.CreateRoom
{
    public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelForRoomSetupAsync(request.HotelId, request.BlockId, request.FloorId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            var block = hotel.Blocks.FirstOrDefault();
            if (block == null) return Result.Failure<Guid>(new Error("Block.NotFound", "Block does not match or does not exist."));

            var floor = block.Floors.FirstOrDefault();
            if (floor == null) return Result.Failure<Guid>(new Error("Floor.NotFound", "Floor does not match or does not exist."));

            try
            {
                floor.AddRoom(request.Name, request.RoomTypeId);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                var newRoom = floor.Rooms.Last();
                return Result.Success(newRoom.Id);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Guid>(new Error("Room.Invalid", ex.Message));
            }
        }
    }
}
