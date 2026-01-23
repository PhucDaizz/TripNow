using BookingService.Application.DTOs.Booking;
using BookingService.Domain.Common.Models;
using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Queries.GetBookings
{
    public class GetBookingsQuery : IRequest<Result<PagedResult<BookingSummaryDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Basic Filters
        public BookingStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        // Advanced Filters
        public Guid? HotelId { get; set; } // Admin có thể lọc theo HotelId
        public Guid? BookingId { get; set; } // Tìm chính xác mã đơn
        public DateOnly? FromDate { get; set; } // Lọc theo ngày CheckIn
        public DateOnly? ToDate { get; set; }
        public string? CustomerEmail { get; set; } // Tìm theo email khách (dành cho Admin/Staff)
        public string? CustomerName { get; set; }
    }
}
