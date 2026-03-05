using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using BookingService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMovedToAnotherRoomTypeEventHandler : INotificationHandler<RoomMovedToAnotherRoomTypeEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;
        private readonly ILogger<RoomMovedToAnotherRoomTypeEventHandler> _logger;

        public RoomMovedToAnotherRoomTypeEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings, ILogger<RoomMovedToAnotherRoomTypeEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
            _logger = logger;
        }

        public async Task Handle(RoomMovedToAnotherRoomTypeEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);

            var oldConfig = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(notification.OldRoomType, cancellationToken);
            var newConfig = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(notification.NewRoomType, cancellationToken);

            if (newConfig == null)
            {
                _logger.LogWarning("Missing InventoryConfiguration for Target RoomType: {RoomTypeId}", notification.NewRoomType);
            }

            var roomTypeIds = new List<Guid> { notification.OldRoomType, notification.NewRoomType };
            var existingInventories = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
                roomTypeIds, today, endDate, cancellationToken);

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    if (oldConfig != null)
                    {
                        int newOldStock = oldConfig.DefaultStock - 1;
                        oldConfig.UpdateDefaultStock(newOldStock < 0 ? 0 : newOldStock);
                    }

                    if (newConfig != null)
                    {
                        newConfig.UpdateDefaultStock(newConfig.DefaultStock + 1);
                    }

                    foreach (var inv in existingInventories)
                    {
                        if (inv.RoomTypeId == notification.OldRoomType)
                        {
                            inv.AdjustTotalStock(inv.TotalStock - 1);
                        }
                        else if (inv.RoomTypeId == notification.NewRoomType)
                        {
                            inv.AdjustTotalStock(inv.TotalStock + 1);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    break; 
                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex, "Cannot change room from {Old} to {New} because the old room type is currently occupied by a guest reservation. Please relocate the guest before making the change.", notification.OldRoomType, notification.NewRoomType);

                    throw;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Continuous data conflict error when transferring rooms from {Old} to {New}", notification.OldRoomType, notification.NewRoomType);
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
