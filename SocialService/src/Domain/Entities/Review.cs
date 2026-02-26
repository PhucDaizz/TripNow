using SocialService.Domain.Common;
using SocialService.Domain.Enum;
using SocialService.Domain.Exceptions;

namespace SocialService.Domain.Entities
{
    public class Review: BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid TargetId { get; private set; }
        public TargetTypeReview TargetType { get; set; }
        public decimal Rating { get; set; }
        public Guid? BookingId { get; set; } 

        private Review() { }

        internal Review(Guid postId, Guid targetId, TargetTypeReview targetType, decimal rating, Guid? bookingId)
        {
            if (postId == Guid.Empty) throw new DomainException("PostId không hợp lệ.");
            if (targetId == Guid.Empty) throw new DomainException("TargetId không hợp lệ.");

            ValidateRating(rating);
            ValidateBookingLogic(targetType, bookingId);

            PostId = postId;
            TargetId = targetId;
            TargetType = targetType;
            Rating = rating;
            BookingId = bookingId;
        }

        /// <summary>
        /// Cho phép người dùng sửa lại điểm đánh giá
        /// </summary>
        internal void UpdateRating(decimal newRating)
        {
            ValidateRating(newRating);
            Rating = newRating;
            // (Optional) Có thể update UpdatedAt nếu BaseEntity có trường này
        }


        private void ValidateRating(decimal rating)
        {
            // Giả sử thang điểm 1-5
            if (rating < 1 || rating > 5)
            {
                throw new DomainException("Điểm đánh giá phải từ 1 đến 5.");
            }
        }

        private void ValidateBookingLogic(TargetTypeReview type, Guid? bookingId)
        {
            // Rule: Nếu review Hotel thì BẮT BUỘC phải có BookingId
            if (type == TargetTypeReview.Hotel && (bookingId == null || bookingId == Guid.Empty))
            {
                throw new DomainException("Review khách sạn bắt buộc phải có mã đặt phòng (BookingId) để xác thực.");
            }

        }
    }
}
