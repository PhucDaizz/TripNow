using BookingService.Domain.Entities;

namespace BookingService.Domain.Repositories
{
    public interface IBookingPriceSnapshotRepository
    {
        Task<BookingPriceSnapshot?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task AddAsync(BookingPriceSnapshot bookingPriceSnapshot, CancellationToken cancellationToken = default);
        Task Update(BookingPriceSnapshot bookingPriceSnapshot);
    }
}
