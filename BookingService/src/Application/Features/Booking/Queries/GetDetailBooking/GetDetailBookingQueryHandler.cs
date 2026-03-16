using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
using BookingService.Domain.Common;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Booking.Queries.GetDetailBooking
{
    public class GetDetailBookingQueryHandler : IRequestHandler<GetDetailBookingQuery, Result<BookingDetailResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public GetDetailBookingQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IHotelAuthorizationService hotelAuthService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result<BookingDetailResponse>> Handle(GetDetailBookingQuery request, CancellationToken cancellationToken)
        {

            if(request.UserId == null)
            {
                return Result.Failure<BookingDetailResponse>(new Error("NO.LOGIN","UserId cannot be null"));
            }

            var booking = await _context.Booking
            .AsNoTracking()
            .Where(b => b.Id == request.BookingId)
            .Select(b => new BookingDetailResponse
            {
                Id = b.Id,
                HotelId = b.HotelId,
                UserId = b.UserId,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Status = b.Status.ToString(),
                PaymentStatus = b.PaymentStatus.ToString(),
                TotalAmount = b.TotalAmount,
                DiscountAmount = b.DiscountAmount,
                Items = b.Items.Select(i => new BookingItemDetailDTO
                {
                    RoomTypeId = i.RoomTypeId,
                    Quantity = i.Quantity, 
                    UnitPrice = i.Price
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return Result.Failure<BookingDetailResponse>(
                    new Error("Booking.NotFound", "Booking not found"));

            if (!await CanViewBooking(booking, cancellationToken))
            {
                return Result.Failure<BookingDetailResponse>(
                    new Error("Booking.AccessDenied", "You do not have permission to view this booking"));
            }

            return Result.Success(booking);
        }

        private async Task<bool> CanViewBooking(BookingDetailResponse booking, CancellationToken cancellationToken)
        {
            if (_currentUserService.Role == AppRoles.Customer)
            {
                return string.Equals(booking.UserId.ToString(), _currentUserService.UserId, StringComparison.OrdinalIgnoreCase);
            }

            return await _hotelAuthService.HasHotelAccessAsync(booking.HotelId, cancellationToken);
        }

    }
}
