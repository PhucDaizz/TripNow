using BookingService.Domain.Entities;
using BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBookingAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            await _context.Booking.AddAsync(booking, cancellationToken);
        }

        public Task DeleteBookingAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _context.Booking.Remove(booking);
            return Task.CompletedTask;
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Booking.FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        }

        public async Task<Booking?> GetBookingByIdNoTrackingAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Booking
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);
        }

        public async Task<Booking?> GetBookingWithDetailItemAssignmentAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Booking
                .Include(b => b.Items.Where(x => x.BookingId == bookingId))
                    .ThenInclude(i => i.Assignments)
                .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
        }

        public async Task<Booking?> GetBookingWithDetailItemAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Booking
                .Include(b => b.Items.Where(x => x.BookingId == bookingId))
                .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
        }

        public Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _context.Booking.Update(booking);
            return Task.CompletedTask;
        }
    }
}
