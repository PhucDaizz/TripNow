namespace SocialService.Application.Contracts
{
    public interface IBookingService
    {
        Task<bool> IsBookingExisting(Guid bookingId, Guid userId, CancellationToken cancellationToken = default);
    }
}
