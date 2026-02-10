namespace HotelCatalogService.Application.Contracts
{
    public interface IBookingService
    {
        Task<bool> CheckIsHaveAnyBookInFunitue(Guid RoomTypeId, CancellationToken token = default);
    }
}
