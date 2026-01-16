using BookingService.Application.DTOs.HotelCatalog;

namespace BookingService.Application.Contracts
{
    public interface IHotelCatalogService
    {
        Task<PromotionValidationResult> ValidatePromotionAsync(Guid hotelId, string code, decimal totalBaseAmount, Guid userId, CancellationToken token = default);
        Task<PromotionApplyResult> ApplyPromotionAsync(Guid hotelId, string code, decimal orderAmount, Guid userId, Guid bookingId, CancellationToken token = default);
        Task<List<CatalogBatchPriceDto>> GetBatchRoomPricesAsync(Guid hotelId, DateOnly fromDate, DateOnly toDate, CancellationToken token = default);
    }
}
