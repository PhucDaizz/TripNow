using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.UpdateFloor
{
    public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFloorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateFloorCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            var block = hotel.Blocks.FirstOrDefault(b => b.Id == request.BlockId);
            if (block == null) return Result.Failure(new Error("Block.NotFound", "Block not found"));

            try
            {
                block.UpdateFloor(request.FloorId, request.FloorNumber);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error("Floor.Invalid", ex.Message));
            }
        }
    }
}
