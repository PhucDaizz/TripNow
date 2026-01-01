using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.ChangePromotionStatus
{
    public class ChangePromotionStatusCommandHandler : IRequestHandler<ChangePromotionStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePromotionStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangePromotionStatusCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithPromotionsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "Forbidden"));

            try
            {
                hotel.ChangePromotionStatus(request.PromotionId, request.IsActive);

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Promotion.StatusError", ex.Message));
            }
        }
    }
}
