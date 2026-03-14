using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomCheckedOut
{
    public class RoomCheckedOutEventHandler : INotificationHandler<RoomCheckedOutEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;
        private readonly IHousekeepingSignalRService _signalRService;

        public RoomCheckedOutEventHandler(
            IUnitOfWork unitOfWork, 
            IApplicationDbContext context,
            IHousekeepingSignalRService signalRService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _signalRService = signalRService;
        }

        public async Task Handle(RoomCheckedOutEvent notification, CancellationToken cancellationToken)
        {
            var room = await _context.Room.FirstOrDefaultAsync(x => x.Id == notification.RoomId, cancellationToken);

            if (room == null)
            {
                return;
            }

            room.CheckOut();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var floor = await _context.Floor.FirstOrDefaultAsync(f => f.Id == room.FloorId, cancellationToken);
            var block = await _context.Block.FirstOrDefaultAsync(b => b.Id == floor.BlockId, cancellationToken);

            Guid hotelId = block!.HotelId;

            var updateDto = new RoomStatusUpdateDto
            {
                RoomId = room.Id,
                RoomName = room.RoomName ?? "Phòng",

                BlockId = block?.Id ?? Guid.Empty,
                BlockName = block?.Name ?? "Khu vực",
                FloorId = room.FloorId,
                FloorName = floor?.FloorNumber.ToString() ?? "Lầu",

                Status = RoomStatus.Dirty.ToString(), 

                AssignedToStaffId = null,
                AssignedToStaffName = null
            };

            await _signalRService.BroadcastRoomStatusAsync(hotelId, updateDto);
        }
    }
}
