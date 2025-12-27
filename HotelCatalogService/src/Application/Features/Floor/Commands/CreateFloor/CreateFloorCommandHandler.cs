using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Commands.CreateFloor
{
    public class CreateFloorCommandHandler : IRequestHandler<CreateFloorCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFloorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateFloorCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Not permitted"));

            var block = hotel.Blocks.FirstOrDefault(b => b.Id == request.BlockId);
            if (block == null) return Result.Failure<Guid>(new Error("Block.NotFound", "Not found block"));

            try
            {
                block.AddFloor(request.FloorNumber);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                var newFloor = block.Floors.First(f => f.FloorNumber == request.FloorNumber);
                return Result.Success(newFloor.Id);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Guid>(new Error("Floor.Invalid", ex.Message));
            }
        }
    }
}
