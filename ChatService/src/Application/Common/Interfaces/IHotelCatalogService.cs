namespace ChatService.Application.Common.Interfaces
{
    public interface IHotelCatalogService
    {
        Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken);
    }
}
