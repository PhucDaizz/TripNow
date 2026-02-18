using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Queries.IsBookingExisting
{
    public class IsBookingExistingQuery: IRequest<Result<bool>>
    {
        public Guid BookingId { get; set; }
    }
}
