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
            const int maxRetries = 3;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var inventories = await _unitOfWork.Inventory.GetInventoriesByDateRangeAsync(
                        notification.RoomTypeId,
                        notification.FromDate,
                        notification.ToDate,
                        cancellationToken);

                    foreach (var inv in inventories)
                    {
                        inv.BlockStock(1);
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    break; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retry == maxRetries - 1)
                        throw;

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync(cancellationToken);
                    }

                    await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);
                }
            }
        }
    }
}
