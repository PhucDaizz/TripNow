using System;
using RecommendationService.Domain.Common;

namespace RecommendationService.Domain.Entities
{
    public class UserViewedHotel : BaseEntity
    {
        public Guid UserId { get; private set; } // Ai đang xem
        public Guid HotelId { get; private set; } // Khách sạn nào được click vào xem chi tiết
        public DateTime ViewedAt { get; private set; }

        // EF Core binding
        protected UserViewedHotel() { }

        private UserViewedHotel(Guid userId, Guid hotelId, DateTime viewedAt)
        {
            UserId = userId;
            HotelId = hotelId;
            ViewedAt = viewedAt;
        }

        public static UserViewedHotel Create(Guid userId, Guid hotelId, DateTime viewedAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            if (hotelId == Guid.Empty)
                throw new ArgumentException("HotelId cannot be empty.", nameof(hotelId));

            if (viewedAt == DateTime.MinValue)
                throw new ArgumentException("ViewedAt must be a valid date and time.", nameof(viewedAt));

            return new UserViewedHotel(userId, hotelId, viewedAt);
        }

        public void UpdateViewTime(DateTime latestViewedAt)
        {
            ViewedAt = latestViewedAt;
        }

    }
}
