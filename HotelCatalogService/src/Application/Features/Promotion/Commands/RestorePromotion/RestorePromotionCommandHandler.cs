using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.RestorePromotion
{
    public class RestorePromotionCommandHandler : IRequestHandler<RestorePromotionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestorePromotionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RestorePromotionCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBookingPromotionUsageAsync(request.HotelId, request.PromotionCode, request.BookingId, token);

            if (hotel == null)
            {
                return Result.Success();
            }

            try
            {
                hotel.RefundPromotion(request.BookingId);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Promotion.RestoreFailed", ex.Message));
            }
        }
    }
}
