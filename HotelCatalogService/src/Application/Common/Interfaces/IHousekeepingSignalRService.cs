using HotelCatalogService.Application.DTOs.Room;

namespace HotelCatalogService.Application.Common.Interfaces
{
    public interface IHousekeepingSignalRService
    {
        Task BroadcastRoomStatusAsync(Guid hotelId, RoomStatusUpdateDto updateData);
    }
}
