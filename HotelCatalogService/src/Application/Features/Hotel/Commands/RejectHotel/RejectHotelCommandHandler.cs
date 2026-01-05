using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.RejectHotel
{
    public class RejectHotelCommandHandler : IRequestHandler<RejectHotelCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectHotelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RejectHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdAsync(request.HotelId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure(new Error("Hotel.NotFound", "No hotel found."));
            }

            if(request.AdminId == null)
            {
                return Result.Failure(new Error("Hotel.InvalidAdmin", "AdminId is required to reject a hotel."));
            }

            hotel.Reject(request.Reason, request.AdminId.Value);

            await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}
