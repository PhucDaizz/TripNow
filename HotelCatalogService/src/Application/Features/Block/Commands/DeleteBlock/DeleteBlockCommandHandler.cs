
using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Commands.DeleteBlock
{
    public class DeleteBlockCommandHandler : IRequestHandler<DeleteBlockCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBlockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteBlockCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            try
            {
                hotel.RemoveBlock(request.BlockId);
                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error("Block.CannotDelete", ex.Message));
            }
        }
    }
}
