namespace ChatService.Application.Common.Interfaces
{
    public interface IHotelAuthorizationService
    {
        /// <summary>
        /// Kiểm tra xem User hiện tại có quyền quản lý Khách sạn này hay không 
        /// </summary>
        Task<bool> HasHotelAccessAsync(Guid hotelId, CancellationToken cancellationToken = default);
    }
}
