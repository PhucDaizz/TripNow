using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.HubInterfaces;
using HotelCatalogService.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HotelCatalogService.Infrastructure.SignalR
{
    public class HousekeepingSignalRService : IHousekeepingSignalRService
    {
        private readonly IHubContext<HousekeepingHub, IHousekeepingClient> _hubContext;

        public HousekeepingSignalRService(IHubContext<HousekeepingHub, IHousekeepingClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastRoomStatusAsync(Guid hotelId, RoomStatusUpdateDto updateData)
        {
            await _hubContext.Clients.Group($"Housekeeping_Hotel_{hotelId}")
                                     .ReceiveRoomStatusUpdate(updateData);
        }
    }
}
