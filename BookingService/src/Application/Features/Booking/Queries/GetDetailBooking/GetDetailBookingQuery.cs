using BookingService.Application.DTOs.Booking;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Queries.GetDetailBooking
{
    public class GetDetailBookingQuery: IRequest<Result<BookingDetailResponse>>
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
    }
}
