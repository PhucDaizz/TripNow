namespace SocialService.Application.Contracts
{
    public interface IHotelCatalogService
    {
        Task<bool> IsHotelExisting(Guid hotelId, CancellationToken cancellationToken = default);
    }
}
