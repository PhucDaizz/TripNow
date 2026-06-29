using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.BookingPriceDetail;
using BookingService.Domain.Common;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.BookingPriceDetail.Queries.GetPriceDetails
{
    public class GetBookingPriceDetailsQueryHandler : IRequestHandler<GetBookingPriceDetailsQuery, Result<List<BookingPriceDetailDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingPriceDetailsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<BookingPriceDetailDto>>> Handle(GetBookingPriceDetailsQuery request, CancellationToken token)
        {
            var bookingAuthInfo = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == request.BookingId)
                .Select(b => new { b.Id, b.HotelId, b.UserId })
                .FirstOrDefaultAsync(token);

            if (bookingAuthInfo == null)
                return Result.Failure<List<BookingPriceDetailDto>>(new Error("Booking.NotFound", "Booking not found."));

            if (!CanViewBooking(bookingAuthInfo.HotelId, bookingAuthInfo.UserId))
            {
                return Result.Failure<List<BookingPriceDetailDto>>(new Error("Auth.Forbidden", "Access denied."));
            }

            var details = await _context.BookingPriceDetails 
                .AsNoTracking()
                .Where(x => x.BookingId == request.BookingId)
                .Select(x => new BookingPriceDetailDto
                {
                    Id = x.Id,
                    Type = x.Type.ToString(), 
                    Amount = x.Amount,
                    Description = x.Description
                })
                .ToListAsync(token);

            return Result.Success(details);
        }

        private bool CanViewBooking(Guid bookingHotelId, Guid bookingUserId)
        {
            var role = _currentUserService.Role;
            var currentUserId = _currentUserService.UserId;
            var currentHotelId = _currentUserService.HotelId;

            return role switch
            {
                AppRoles.SysAdmin => true,
                AppRoles.HotelOwner or AppRoles.Receptionist => bookingHotelId == currentHotelId,
                AppRoles.Customer => bookingUserId.ToString() == currentUserId,
                _ => false
            };
        }
    }
}
