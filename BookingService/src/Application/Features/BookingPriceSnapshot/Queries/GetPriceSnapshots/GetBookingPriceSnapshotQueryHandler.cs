using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.BookingPriceSnapshot;
using BookingService.Domain.Common;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.BookingPriceSnapshot.Queries.GetPriceSnapshots
{
    public class GetBookingPriceSnapshotQueryHandler : IRequestHandler<GetBookingPriceSnapshotQuery, Result<BookingPriceSnapshotDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingPriceSnapshotQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<BookingPriceSnapshotDto>> Handle(GetBookingPriceSnapshotQuery request, CancellationToken token)
        {
            var bookingAuthInfo = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == request.BookingId)
                .Select(b => new { b.Id, b.HotelId, b.UserId })
                .FirstOrDefaultAsync(token);

            if (bookingAuthInfo == null)
                return Result.Failure<BookingPriceSnapshotDto>(new Error("Booking.NotFound", "Booking not found."));

            if (!CanViewBooking(bookingAuthInfo.HotelId, bookingAuthInfo.UserId))
                return Result.Failure<BookingPriceSnapshotDto>(new Error("Auth.Forbidden", "Access denied."));

            var snapshot = await _context.BookingPriceSnapshots
                .AsNoTracking()
                .Where(x => x.BookingId == request.BookingId)
                .Select(x => new BookingPriceSnapshotDto
                {
                    Id = x.Id,
                    BookingId = x.BookingId,

                    GrossAmount = x.GrossAmount,
                    PromotionAmount = x.PromotionAmount,
                    VATAmount = x.VATAmount,
                    ServiceFeeAmount = x.ServiceFeeAmount,
                    NetPayableByCustomer = x.NetPayableByCustomer,

                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(token);

            if (snapshot == null)
                return Result.Failure<BookingPriceSnapshotDto>(new Error("Snapshot.NotFound", "Invoice has not been generated yet."));

            return Result.Success(snapshot);
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
