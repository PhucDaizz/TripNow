using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class AddRoomInventoryEventHandler : INotificationHandler<AddRoomInventoryEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventorySettings _inventorySettings;
        private readonly ILogger<AddRoomInventoryEventHandler> _logger;

        public AddRoomInventoryEventHandler(IUnitOfWork unitOfWork, IInventorySettings inventorySettings, ILogger<AddRoomInventoryEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _inventorySettings = inventorySettings;
            _logger = logger;
        }

        public async Task Handle(AddRoomInventoryEvent notification, CancellationToken cancellationToken)
        {
            var roomTypeId = notification.RoomtypeId;

            var config = await _unitOfWork.InventoryConfiguration.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
            if (config == null)
            {
                _logger.LogWarning("Inventory configuration not found for RoomTypeId: {RoomTypeId}", roomTypeId);
            }

            int currentDefaultStock = config?.DefaultStock ?? 0;
            int newTotalStock = currentDefaultStock + 1;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(_inventorySettings.LookAheadDays);

            var existingInventories = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
                new List<Guid> { roomTypeId }, today, endDate, cancellationToken);

            var existingDatesSet = existingInventories.Select(i => i.Date).ToHashSet();
            var newInventories = new List<Domain.Entities.Inventory>();

            var maxGeneratedDate = config?.LastGeneratedDate != null
                ? DateOnly.FromDateTime(config.LastGeneratedDate.Value)
                : today.AddDays(-1);

            for (int i = 0; i <= _inventorySettings.LookAheadDays; i++)
            {
                var currentDate = today.AddDays(i);

                if (!existingDatesSet.Contains(currentDate))
                {
                    var newInv = Domain.Entities.Inventory.Create(roomTypeId, currentDate, newTotalStock);
                    newInventories.Add(newInv);

                    if (currentDate > maxGeneratedDate)
                    {
                        maxGeneratedDate = currentDate;
                    }
                }
            }

            foreach (var inv in existingInventories)
            {
                inv.AdjustTotalStock(newTotalStock);
            }

            if (config != null)
            {
                config.UpdateDefaultStock(newTotalStock);

                if (config.LastGeneratedDate == null || maxGeneratedDate > DateOnly.FromDateTime(config.LastGeneratedDate.Value))
                {
                    config.UpdateLastGeneratedDate(maxGeneratedDate);
                }
            }

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    if (retryCount == 0 && newInventories.Any())
                    {
                        await _unitOfWork.Inventory.AddRangeAsync(newInventories, cancellationToken);
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    break; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Room synchronization error causes continuous data conflict for RoomTypeId: {RoomTypeId}", roomTypeId);
                        throw; 
                    }

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();

                        if (entry.Entity is Domain.Entities.Inventory inv)
                        {
                            inv.AdjustTotalStock(newTotalStock);
                        }
                        else if (entry.Entity is Domain.Entities.InventoryConfiguration conf)
                        {
                            conf.UpdateDefaultStock(newTotalStock);
                            if (conf.LastGeneratedDate == null || maxGeneratedDate > DateOnly.FromDateTime(conf.LastGeneratedDate.Value))
                            {
                                conf.UpdateLastGeneratedDate(maxGeneratedDate);
                            }
                        }
                    }

                    await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);
                }
            }
        }
    }
}
