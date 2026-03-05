using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using BookingService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class DeductRoomInventoryEventHandler : INotificationHandler<DeductRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;
        private readonly ILogger<DeductRoomInventoryEventHandler> _logger;

        public DeductRoomInventoryEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings, ILogger<DeductRoomInventoryEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
            _logger = logger;
        }

        public async Task Handle(DeductRoomInventoryEvent notification, CancellationToken cancellationToken)
        {
            var roomTypeId = notification.RoomTypeId;

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
            if (config == null)
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
                return; 
            }

            int currentDefaultStock = config.DefaultStock;
            int newTotalStock = currentDefaultStock - 1;

            if (newTotalStock < 0)
            {
                _logger.LogWarning("Attempt to deduct the room inventory below 0 for RoomTypeId: {RoomTypeId}", roomTypeId);
                return; 
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);

            var existingInventories = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
                new List<Guid> { roomTypeId }, today, endDate, cancellationToken);

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    foreach (var inv in existingInventories)
                    {
                        inv.AdjustTotalStock(newTotalStock);
                    }

                    config.UpdateDefaultStock(newTotalStock);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    break; 
                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex, "Cannot reduce room inventory for RoomTypeId: {RoomTypeId} because there are dates already Sold. Please move the guest to another room before deducting.", roomTypeId);
                    throw; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Continuous data conflict error when reducing room inventory for RoomTypeId: {RoomTypeId}", roomTypeId);
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
