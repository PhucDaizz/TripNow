using HotelCatalogService.Application.HubInterfaces;
using HotelCatalogService.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HotelCatalogService.Infrastructure.Hubs
{
    [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Housekeeping},{AppRoles.SysAdmin}")]
    public class HousekeepingHub : Hub<IHousekeepingClient>
    {
        public override async Task OnConnectedAsync()
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var tokenHotelId = Context.User?.FindFirst("HotelId")?.Value;

            var httpContext = Context.GetHttpContext();
            var queryHotelId = httpContext?.Request.Query["hotelId"].ToString();

            Guid hotelIdToJoin = Guid.Empty;

            if (role == AppRoles.Housekeeping && Guid.TryParse(tokenHotelId, out var hId))
            {
                hotelIdToJoin = hId; // Lễ tân/Dọn phòng lấy thẳng từ Token
            }
            else if ((role == AppRoles.HotelOwner || role == AppRoles.SysAdmin) && Guid.TryParse(queryHotelId, out var qId))
            {
                hotelIdToJoin = qId; // Owner/Admin lấy từ QueryString
            }

            // 3. Cho vào Group
            if (hotelIdToJoin != Guid.Empty)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Housekeeping_Hotel_{hotelIdToJoin}");
            }

            await base.OnConnectedAsync();
        }
    }
}
