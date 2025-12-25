
using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;
using System.Security;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CreateRoomType
{
    public class CreateRoomTypeCommandHandler : IRequestHandler<CreateRoomTypeCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateRoomTypeCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.RoomTypes);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found hotel"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "No permission"));

            hotel.DefineRoomType(request.Name, request.BasePrice, request.Capacity, request.SizeM2);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            var newRoomType = hotel.RoomTypes.Last();

            return Result.Success(newRoomType.Id);
        }
    }
}
