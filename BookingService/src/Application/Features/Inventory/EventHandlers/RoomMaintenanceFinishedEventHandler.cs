using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMaintenanceFinishedEventHandler : INotificationHandler<RoomMaintenanceFinishedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomMaintenanceFinishedEventHandler> _logger;

        public RoomMaintenanceFinishedEventHandler(IUnitOfWork unitOfWork, ILogger<RoomMaintenanceFinishedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(RoomMaintenanceFinishedEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            DateOnly releaseFromDate = notification.FromDate > today
                ? notification.FromDate
                : today;

            if (releaseFromDate > notification.ToDate)
            {
                return;
            }

            var roomTypeId = notification.RoomTypeId;

            var existingInventories = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
                new List<Guid> { roomTypeId },
                releaseFromDate,
                notification.ToDate,
                cancellationToken);

            if (!existingInventories.Any())
            {
                return; 
            }

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    foreach (var inv in existingInventories)
                    {
                        inv.UnblockStock(1);
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    break; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Continuous data collision error when unlocking the maintenance room for RoomTypeId: {RoomTypeId}", roomTypeId);
                        throw;
                    }

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }

                    await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);
                }
            }
        }
    }
}
