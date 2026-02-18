using BookingService.Application.Common.Interfaces;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Queries.IsBookingExisting
{
    public class IsBookingExistingQueryHandler : IRequestHandler<IsBookingExistingQuery, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IsBookingExistingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(IsBookingExistingQuery request, CancellationToken cancellationToken)
        {
            var isExisting = await _unitOfWork.Booking.IsBookingExistingAsync(request.BookingId, cancellationToken);
            return Result.Success(isExisting);
        }
    }
}
