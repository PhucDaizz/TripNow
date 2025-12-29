using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.DeleteRoom
{
    public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelForRoomSetupAsync(request.HotelId, request.BlockId, request.FloorId, token);
            
            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            var block = hotel.Blocks.FirstOrDefault();
            if (block == null) return Result.Failure<Guid>(new Error("Block.NotFound", "Block does not match or does not exist."));

            var floor = block.Floors.FirstOrDefault();
            if (floor == null) return Result.Failure<Guid>(new Error("Floor.NotFound", "Floor does not match or does not exist."));


            if (floor == null) return Result.Failure(new Error("Floor.NotFound", "Not found"));

            floor.RemoveRoom(request.RoomId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
