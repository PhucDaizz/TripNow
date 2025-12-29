using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.BookingExpired
{
    public class BookingExpiredEventHandler : INotificationHandler<BookingExpiredEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingExpiredEventHandler> _logger;

        public BookingExpiredEventHandler(
            IUnitOfWork unitOfWork,
            ILogger<BookingExpiredEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(BookingExpiredEvent notification, CancellationToken token)
        {
            _logger.LogInformation($"[Event] Handling expired booking for Room {notification.RoomId}");

            // 1. Tìm Hotel chứa Room đó (Dùng hàm tìm ngược đã viết ở bài trước)
            var hotel = await _unitOfWork.Hotel.GetHotelWithSpecificRoomAsync(notification.HotelId,notification.RoomId, token);

            if (hotel == null)
            {
                _logger.LogWarning($"Hotel {notification.HotelId} not found.");
                return;
            }

            var room = hotel.Blocks.FirstOrDefault()
                        ?.Floors.FirstOrDefault()
                        ?.Rooms.FirstOrDefault();

            if (room == null)
            {
                _logger.LogWarning($"Room {notification.RoomId} not found in Hotel {notification.HotelId}.");
                return;
            }

            try
            {
                if (room.Status != RoomStatus.Dirty)
                {
                    room.MarkAsDirty();

                    await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                    await _unitOfWork.SaveChangesAsync(token);

                    _logger.LogInformation($"[Booking Expired] Room {room.RoomName} marked as DIRTY.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update room status");
            }
        }
    }
}
