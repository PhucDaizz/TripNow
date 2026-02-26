using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
using BookingService.Domain.Common;
using BookingService.Domain.Common.Models;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Booking.Queries.GetBookings
{
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, Result<PagedResult<BookingSummaryDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public GetBookingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IHotelAuthorizationService hotelAuthService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result<PagedResult<BookingSummaryDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Booking
                .AsNoTracking()
                .Include(b => b.Items)
                .AsQueryable();

            var role = _currentUserService.Role;
            var currentUserId = Guid.Parse(_currentUserService.UserId);

            switch (role)
            {
                case AppRoles.Customer:
                    query = query.Where(b => b.UserId == currentUserId);
                    break;

                case AppRoles.HotelOwner:
                    Guid? targetHotelId = _currentUserService.HotelId ?? request.HotelId;
                    if (targetHotelId == null)
                    {
                        return Result.Failure<PagedResult<BookingSummaryDto>>(
                            new Error("Auth.NoHotel", "Please provide the HotelId to view the booking list."));
                    }
                    bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(targetHotelId.Value, cancellationToken);
                    if (!hasAccess)
                    {
                        return Result.Failure<PagedResult<BookingSummaryDto>>(
                            new Error("Auth.Forbidden", "You do not have permission to access this hotel's order data."));
                    }
                    query = query.Where(b => b.HotelId == targetHotelId.Value);
                    break;
                case AppRoles.Receptionist:
                    var staffHotelId = _currentUserService.HotelId;
                    if (staffHotelId == null)
                        return Result.Failure<PagedResult<BookingSummaryDto>>(new Error("Auth.NoHotel", "Staff not assigned to any hotel."));

                    query = query.Where(b => b.HotelId == staffHotelId);
                    break;

                case AppRoles.SysAdmin:
                    if (request.HotelId.HasValue)
                    {
                        query = query.Where(b => b.HotelId == request.HotelId.Value);
                    }
                    break;

                default:
                    return Result.Failure<PagedResult<BookingSummaryDto>>(new Error("Auth.InvalidRole", "Unauthorized access."));
            }

            if (request.BookingId.HasValue)
            {
                query = query.Where(b => b.Id == request.BookingId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(b => b.Status == request.Status.Value);
            }

            if (request.PaymentStatus.HasValue)
            {
                query = query.Where(b => b.PaymentStatus == request.PaymentStatus.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate >= request.FromDate.Value);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate <= request.ToDate.Value);
            }
            
            if (request.FromCheckOutDate.HasValue)
            {
                query = query.Where(b => b.CheckOutDate >= request.FromCheckOutDate.Value);
            }

            if (request.ToCheckOutDate.HasValue)
            {
                query = query.Where(b => b.CheckOutDate <= request.ToCheckOutDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                query = query.Where(b => b.CustomerEmail.Contains(request.CustomerEmail));
            }

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
            {
                query = query.Where(b => b.CustomerName.Contains(request.CustomerName));
            }

            query = query.OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookingSummaryDto
                {
                    Id = b.Id,
                    HotelId = b.HotelId,
                    UserId = b.UserId,
                    HotelName = b.HotelName,
                    CustomerName = b.CustomerName,
                    CustomerEmail = b.CustomerEmail,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    Status = b.Status.ToString(),
                    PaymentStatus = b.PaymentStatus.ToString(),
                    TotalAmount = b.TotalAmount,
                    TotalItems = b.Items.Count,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<BookingSummaryDto>.Create(items, totalCount, request.PageNumber, request.PageSize);

            return Result.Success(pagedResult);
        }
    }
}