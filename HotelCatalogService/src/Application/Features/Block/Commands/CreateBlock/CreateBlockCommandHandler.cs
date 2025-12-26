using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Commands.CreateBlock
{
    public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBlockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateBlockCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.Blocks);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Hotel not found!"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Not permitted"));

            try
            {
                hotel.AddBlock(request.Name);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                var newBlock = hotel.Blocks.Last();
                return Result.Success(newBlock.Id);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Guid>(new Error("Block.Duplicate", ex.Message));
            }
        }
    }
}
