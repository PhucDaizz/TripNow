using BookingService.Application.DTOs.HotelCatalog;

namespace BookingService.Application.Contracts
{
    public interface IHotelCatalogService
    {
        Task<PromotionValidationResult> ValidatePromotionAsync(Guid hotelId, string code, decimal totalBaseAmount, Guid userId, CancellationToken token = default);
        Task<PromotionApplyResult> ApplyPromotionAsync(Guid hotelId, string code, decimal orderAmount, Guid userId, Guid bookingId, CancellationToken token = default);
        Task<List<CatalogBatchPriceDto>> GetBatchRoomPricesAsync(Guid hotelId, DateOnly fromDate, DateOnly toDate, CancellationToken token = default);
        Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default);
        Task<RoomResponse?> CheckInRoom(Guid hotelId, Guid roomId, Guid checkInBy, CancellationToken token = default);
        Task RollbackCheckInRoom(Guid hotelId, Guid roomId, CancellationToken token);
        Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken);
    }
}
