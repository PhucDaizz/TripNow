using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.DeleteFloor
{
    public class DeleteFloorCommandHandler : IRequestHandler<DeleteFloorCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFloorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteFloorCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            var block = hotel.Blocks.FirstOrDefault(b => b.Id == request.BlockId);
            if (block == null) return Result.Failure(new Error("Block.NotFound", "Block not found"));

            bool hasRooms = await _unitOfWork.Hotel.HasRoomsInFloorAsync(request.FloorId, token);

            if (hasRooms)
            {
                return Result.Failure(new Error("Floor.HasRooms", "Không thể xoá tầng này vì vẫn còn phòng bên trong. Vui lòng xoá hoặc chuyển các phòng sang tầng khác trước."));
            }

            try
            {
                block.RemoveFloor(request.FloorId);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error("Floor.CannotDelete", ex.Message));
            }
        }
    }
}
