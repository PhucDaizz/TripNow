using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.UpdatePromotion
{
    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromotionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdatePromotionCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithPromotionsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            try
            {
                hotel.UpdatePromotion(
                    request.PromotionId,
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
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Promotion.UpdateFailed", ex.Message));
            }
        }
    }
}
