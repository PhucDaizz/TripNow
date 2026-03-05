using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Events.Booking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Booking.EventHandlers.BookingCancelled
{
    public class BookingCancelledDomainEventHandler : INotificationHandler<BookingCancelledDomainEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingCancelledDomainEventHandler> _logger;

        public BookingCancelledDomainEventHandler(IUnitOfWork unitOfWork, ILogger<BookingCancelledDomainEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(BookingCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            var roomTypeIds = notification.Items
            .Select(i => i.RoomTypeId)
            .Distinct()
            .ToList();

            int maxRetries = 3;

            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    var inventories = await _unitOfWork.Inventory
                        .GetInventoriesInRangeAsync(
                            roomTypeIds,
                            notification.Fromdate,
                            notification.ToDate,
                            cancellationToken);

                    var itemLookup = notification.Items
                        .GroupBy(i => i.RoomTypeId)
                        .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity)); 

                    foreach (var inventory in inventories)
                    {
                        if (itemLookup.TryGetValue(inventory.RoomTypeId, out int quantityToRelease))
                        {
                            inventory.ReleaseStock(quantityToRelease);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    break; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError($"[CRITICAL] Lỗi nhả phòng cho Booking");
                        throw;
                    }

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }

                    await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
