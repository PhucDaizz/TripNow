using HotelCatalogService.Application.DTOs.Room;

namespace HotelCatalogService.Application.HubInterfaces
{
    public interface IHousekeepingClient
    {
        Task ReceiveRoomStatusUpdate(RoomStatusUpdateDto updateData);
    }
}
