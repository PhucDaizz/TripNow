using BookingService.Domain.Entities;

namespace BookingService.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<Booking?> GetBookingWithDetailItemAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<Booking?> GetBookingWithDetailItemAssignmentAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<Booking?> GetBookingByIdNoTrackingAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task AddBookingAsync(Booking booking, CancellationToken cancellationToken = default);
        Task DeleteBookingAsync(Booking booking, CancellationToken cancellationToken = default);
        Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
        Task<bool> HaveAnyBookInFuture(Guid roomTypeId, CancellationToken cancellationToken = default);
    }
}
