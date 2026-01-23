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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetDetailBookingQueryHandler(IUnitOfWork unitOfWork, IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _currentUserService = currentUserService;
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

            if (!CanViewBooking(booking))
            {
                return Result.Failure<BookingDetailResponse>(
                    new Error("Booking.AccessDenied", "You do not have permission to view this booking"));
            }

            return Result.Success(booking);
        }

        private bool CanViewBooking(BookingDetailResponse booking)
        {
            return _currentUserService.Role switch
            {
                AppRoles.SysAdmin => true,

                AppRoles.HotelOwner =>
                    booking.HotelId == _currentUserService.HotelId,

                AppRoles.Receptionist =>
                    booking.HotelId == _currentUserService.HotelId,

                AppRoles.Customer =>
                    booking.UserId.ToString() == _currentUserService.UserId,

                _ => false
            };
        }

    }
}
