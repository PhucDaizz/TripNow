using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMaintenanceScheduledEventHandler : INotificationHandler<RoomMaintenanceScheduledEvent>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public RoomMaintenanceScheduledEventHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RoomMaintenanceScheduledEvent notification, CancellationToken cancellationToken)
        {
            var inventories = await _context.Inventory.AsQueryable()
            .Where(x => x.RoomTypeId == notification.RoomTypeId
                        && x.Date >= notification.FromDate
                        && x.Date <= notification.ToDate)
            .ToListAsync(cancellationToken);

            foreach (var inv in inventories)
            {
                inv.BlockStock(1);
            }

            // Lưu ý: Nếu ngày đó chưa có Inventory thì có thể bỏ qua 
            // hoặc logic Create nếu cần (tùy chiến lược của bạn, thường bảo trì tương lai thì inventory đã có rồi)

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
