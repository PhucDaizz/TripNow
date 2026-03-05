using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using BookingService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class InventoryStockChangedEventHandler : INotificationHandler<InventoryStockChangedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;
        private readonly ILogger<InventoryStockChangedEventHandler> _logger;

        public InventoryStockChangedEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings, ILogger<InventoryStockChangedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
            _logger = logger;
        }

        public async Task Handle(InventoryStockChangedEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);
            var roomTypeId = notification.RoomTypeId;
            var quantityChange = notification.QuantityChange; 

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
            if (config == null)
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
                return;
            }

            int newDefaultStock = config.DefaultStock + quantityChange;
            if (newDefaultStock < 0) newDefaultStock = 0;

            var existingInventories = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
                new List<Guid> { roomTypeId }, today, endDate, cancellationToken);
            var existingDatesSet = existingInventories.Select(i => i.Date).ToHashSet();

            var newInventories = new List<Domain.Entities.Inventory>();
            for (int i = 0; i <= _inventorySettings.LookAheadDays; i++)
            {
                var currentDate = today.AddDays(i);
                if (!existingDatesSet.Contains(currentDate))
                {
                    if (newDefaultStock > 0)
                    {
                        var newInv = Domain.Entities.Inventory.Create(roomTypeId, currentDate, newDefaultStock);
                        newInventories.Add(newInv);
                    }
                }
            }

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    config.UpdateDefaultStock(newDefaultStock);

                    foreach (var inv in existingInventories)
                    {
                        int updatedStock = inv.TotalStock + quantityChange;

                        inv.AdjustTotalStock(updatedStock < 0 ? 0 : updatedStock);
                    }

                    if (retryCount == 0 && newInventories.Any())
                    {
                        await _unitOfWork.Inventory.AddRangeAsync(newInventories, cancellationToken);
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    break;
                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex, "Error changing room inventory for {RoomTypeId}: Demolishing rooms makes the total room inventory smaller than the number of rooms guests have already booked.", roomTypeId);
                    throw;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Recurring data conflict error when changing room inventory for RoomTypeId: {RoomTypeId}", roomTypeId);
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
