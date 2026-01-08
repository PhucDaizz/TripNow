using BookingService.Application.Common.Interfaces;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null || !Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID.");
        }

        var booking = new Domain.Entities.Booking(
            userId,
            request.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            request.Source,
            userId
        );

        foreach (var item in request.Items)
        {
            booking.AddItem(item.RoomTypeId, item.Quantity, item.UnitPrice);
        }

        _context.Booking.Add(booking);

        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
