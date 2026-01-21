using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CancelBooking
{
    public record CancelBookingCommand: IRequest<Result>
    {
        public Guid BookingId { get; set; }
        public CancelledBy CancelledBy { get; set; }
        public RefundPolicy RefundPolicy { get; set; }
        public string Reason { get; set; }
        public decimal RefundAmount { get; set; } = 0;
    }
}
