using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Features.Hotel.Commands.SubmitForApprovalHotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CloseTemporarilyHotel
{
    public class CloseTemporarilyHotelCommandHandler : IRequestHandler<SubmitForApprovalCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CloseTemporarilyHotelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SubmitForApprovalCommand request, CancellationToken cancellationToken)
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

            hotel.CloseTemporarily();
            await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
