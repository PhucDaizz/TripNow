namespace HotelCatalogService.Application.Contracts
{
    public interface IBookingService
    {
        Task<bool> CheckIsHaveAnyBookInFunitue(Guid roomTypeId, CancellationToken token = default);
    }
}
