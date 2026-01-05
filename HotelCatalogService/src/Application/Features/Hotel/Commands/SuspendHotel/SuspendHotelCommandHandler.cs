using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.SuspendHotel
{
    public class SuspendHotelCommandHandler : IRequestHandler<SuspendHotelCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SuspendHotelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SuspendHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdAsync(request.HotelId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure(new Error("Hotel.NotFound", "No hotel found."));
            }

            if (request.AdminId == null)
            {
                return Result.Failure(new Error("Hotel.InvalidAdmin", "AdminId is required to reject a hotel."));
            }

            hotel.Suspend(request.Reason ,request.AdminId.Value);

            await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
