using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.CreatePromotion
{
    public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromotionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePromotionCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithPromotionsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            try
            {
                hotel.AddPromotion(
                    request.Code,
                    request.DiscountType,
                    request.DiscountValue,
                    request.StartDate,
                    request.EndDate,
                    request.Quantity,
                    request.MinBookingAmount
                );

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                return Result.Success(hotel.Promotions.Last().Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(new Error("Promotion.CreateFailed", ex.Message));
            }
        }
    }
}
