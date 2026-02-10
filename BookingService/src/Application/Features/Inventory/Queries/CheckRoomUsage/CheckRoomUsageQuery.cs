using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Inventory.Queries.CheckRoomUsage
{
    public class CheckRoomUsageQuery: IRequest<bool>
    {
        public Guid RoomTypeId { get; set; }
    }
}
