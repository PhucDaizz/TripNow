using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.DeletePromotion
{
    public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromotionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(DeletePromotionCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithPromotionsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "Forbidden"));

            try
            {
                hotel.DeletePromotion(request.PromotionId);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Promotion.DeleteError", ex.Message));
            }
        }
    }
}
