using MediatR;

namespace BookingService.Application.DTOs.Booking.Event
{
    public record PaymentSucceededEvent: INotification
    {
        public Guid BookingId { get; init; }
    }
}
