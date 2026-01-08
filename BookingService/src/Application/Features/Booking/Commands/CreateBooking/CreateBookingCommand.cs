using BookingService.Domain.Enum;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<Guid>
{
    public Guid HotelId { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public List<BookingItemDto> Items { get; init; } = new();
    public BookingSource Source { get; init; } = BookingSource.Web;
}

public record BookingItemDto
{
    public Guid RoomTypeId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
