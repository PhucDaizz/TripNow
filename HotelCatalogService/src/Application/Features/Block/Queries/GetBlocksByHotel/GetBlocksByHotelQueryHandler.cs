using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Block;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Queries.GetBlocksByHotel
{
    public class GetBlocksByHotelQueryHandler : IRequestHandler<GetBlocksByHotelQuery, Result<List<BlockDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBlocksByHotelQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<BlockDto>>> Handle(GetBlocksByHotelQuery request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token); 

            if (hotel == null) return Result.Failure<List<BlockDto>>(new Error("Hotel.NotFound", "Not found"));

            var dtos = hotel.Blocks.Select(b => new BlockDto
            {
                Id = b.Id,
                Name = b.Name,
                FloorCount = b.Floors?.Count ?? 0
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
