using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.UpdateRoom
{
    public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateRoomCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelForRoomSetupAsync(request.HotelId, request.BlockId, request.FloorId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            var block = hotel.Blocks.FirstOrDefault();
            if (block == null) return Result.Failure<Guid>(new Error("Block.NotFound", "Block does not match or does not exist."));

            var floor = block.Floors.FirstOrDefault();
            if (floor == null) return Result.Failure<Guid>(new Error("Floor.NotFound", "Floor does not match or does not exist."));

            var room = hotel?.Blocks.FirstOrDefault(b => b.Id == request.BlockId)
                              ?.Floors.FirstOrDefault(f => f.Id == request.FloorId)
                              ?.Rooms.FirstOrDefault(r => r.Id == request.RoomId);

            if (room == null) return Result.Failure(new Error("Room.NotFound", "Room not found"));

            room.UpdateDetails(request.Name, request.RoomTypeId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
