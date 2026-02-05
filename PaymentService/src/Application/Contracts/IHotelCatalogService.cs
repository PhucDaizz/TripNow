using PaymentService.Application.DTOs.HotelCatalog;

namespace PaymentService.Application.Contracts
{
    public interface IHotelCatalogService
    {
        Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default);
    }
}
