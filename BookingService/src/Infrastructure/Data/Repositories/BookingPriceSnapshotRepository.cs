using BookingService.Domain.Entities;
using BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data.Repositories
{
    public class BookingPriceSnapshotRepository : IBookingPriceSnapshotRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingPriceSnapshotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookingPriceSnapshot bookingPriceSnapshot, CancellationToken cancellationToken = default)
        {
            await _context.BookingPriceSnapshot.AddAsync(bookingPriceSnapshot, cancellationToken);
        }

        public async Task<BookingPriceSnapshot?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.BookingPriceSnapshot.FirstOrDefaultAsync(bps => bps.BookingId == bookingId, cancellationToken);
        }

        public Task Update(BookingPriceSnapshot bookingPriceSnapshot)
        {
            _context.BookingPriceSnapshot.Update(bookingPriceSnapshot);
            return Task.CompletedTask;
        }
    }
}
