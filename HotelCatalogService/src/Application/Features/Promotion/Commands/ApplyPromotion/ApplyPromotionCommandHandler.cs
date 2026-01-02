using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.Commands.ApplyPromotion
{
    public class ApplyPromotionCommandHandler : IRequestHandler<ApplyPromotionCommand, Result<decimal>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplyPromotionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<decimal>> Handle(ApplyPromotionCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var hasUsed = await _unitOfWork.Hotel.IsPromotionUsedByUserAsync(request.HotelId, request.Code, request.UserId, token);
                if (hasUsed)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<decimal>(new Error("Promotion.Used", "You have already used this discount code."));
                }

                var hotel = await _unitOfWork.Hotel.GetHotelWithSpecificPromotionAsync(request.HotelId, request.Code, token);

                if (hotel == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<decimal>(new Error("Promotion.NotFound", "The discount code does not exist."));
                }

                decimal finalDiscountAmount;
                try
                {
                    finalDiscountAmount = hotel.ApplyPromotion(request.Code, request.OrderAmount, request.BookingId, request.HotelId, request.UserId);
                }
                catch (InvalidOperationException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<decimal>(new Error("Promotion.Invalid", ex.Message));
                }

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                await _unitOfWork.CommitTransactionAsync();

                return Result.Success(finalDiscountAmount);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure<decimal>(new Error("System.Error", ex.Message));
            }
        }
    }
}
