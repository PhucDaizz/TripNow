using RecommendationService.Domain.Common;

namespace RecommendationService.Domain.Entities
{
    public class UserSearchHistory : BaseEntity
    {
        public Guid UserId { get; private set; } 

        public string RawQuery { get; private set; } // Ví dụ: "Khách sạn đà lạt có hồ bơi"

        public string? Destination { get; private set; } // Ví dụ: "Đà Lạt"
        public DateTime? CheckInDate { get; private set; }
        public DateTime? CheckOutDate { get; private set; }
        public int? Adults { get; private set; }
        public int? Children { get; private set; }

        public DateTime SearchedAt { get; private set; }

        // EF Core binding
        protected UserSearchHistory() { }

        private UserSearchHistory(
            Guid userId, 
            string rawQuery, 
            string? destination, 
            DateTime? checkInDate, 
            DateTime? checkOutDate, 
            int? adults, 
            int? children)
        {
            UserId = userId;
            RawQuery = rawQuery;
            Destination = destination;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            Adults = adults;
            Children = children;
            SearchedAt = DateTime.UtcNow;
        }

        public static UserSearchHistory Create(
            Guid userId, 
            string rawQuery, 
            string? destination = null, 
            DateTime? checkInDate = null, 
            DateTime? checkOutDate = null, 
            int? adults = null, 
            int? children = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(rawQuery))
                throw new ArgumentException("RawQuery cannot be null or empty.", nameof(rawQuery));

            if (checkInDate.HasValue && checkOutDate.HasValue && checkInDate.Value >= checkOutDate.Value)
                throw new ArgumentException("CheckInDate must be before CheckOutDate.");

            if (adults.HasValue && adults.Value < 0)
                throw new ArgumentException("Adults count cannot be negative.");

            if (children.HasValue && children.Value < 0)
                throw new ArgumentException("Children count cannot be negative.");

            return new UserSearchHistory(
                userId, rawQuery, destination, checkInDate, checkOutDate, adults, children);
        }
    }
}
