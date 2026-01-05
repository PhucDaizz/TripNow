using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.ReopenHotel
{
    public class ReopenHotelCommandHandler : IRequestHandler<ReopenHotelCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReopenHotelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReopenHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdAsync(request.HotelId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure(new Error("Hotel.NotFound", "No hotel found."));
            }

            if (hotel.OwnerId != request.OwerId)
            {
                return Result.Failure(new Error("Hotel.Unauthorized", "You are not authorized to submit this hotel for approval."));
            }

            hotel.Reopen();
            await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
