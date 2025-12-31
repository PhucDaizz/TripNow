using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Commands.BulkSetRoomPrice
{
    public class BulkSetRoomPriceCommandHandler : IRequestHandler<BulkSetRoomPriceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BulkSetRoomPriceCommandHandler(IHotelRepository repo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(BulkSetRoomPriceCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithRoomTypePricesAsync(request.HotelId, request.RoomTypeId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            var roomType = hotel.RoomTypes.FirstOrDefault();
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "RoomType not existing"));

            try
            {
                roomType.SetBulkPrices(request.FromDate, request.ToDate, request.Price, request.SpecificDays);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                return Result.Success();
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(new Error("Validation.Error", ex.Message));
            }
        }
    }
}
