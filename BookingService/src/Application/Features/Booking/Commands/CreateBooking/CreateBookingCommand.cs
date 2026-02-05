using BookingService.Application.DTOs.Booking;
using BookingService.Application.DTOs.Payment;
using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<Result<CreateBookingResponse>>
{
    public Guid HotelId { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public List<BookingItemDto> Items { get; init; } = new();
    public BookingSource Source { get; init; } = BookingSource.Web;
    public Guid? PromotionId { get; init; }
    public string? PromotionCode { get; init; }
    public PaymentProvider paymentProvider { get; init; } = PaymentProvider.VNPay;
}

public record BookingItemDto
{
    public Guid RoomTypeId { get; init; }
    public int Quantity { get; init; }
}
