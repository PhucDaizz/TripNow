using NotificationService.Domain.Common;
using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Entities
{
    public class Notification : BaseEntity, AggregateRoot
    {
        // ID của người nhận (Có thể là CustomerId hoặc HotelOwnerId)
        public Guid OwnerId { get; private set; }

        // Tiêu đề ngắn gọn (VD: "Hoàn tiền thành công")
        public string Title { get; private set; }

        // Nội dung chi tiết (VD: "Đơn đặt phòng XYZ đã được hoàn 500k vào ví")
        public string Message { get; private set; }

        // Phân loại thông báo để Frontend hiển thị Icon tương ứng (Enum)
        public NotificationType Type { get; private set; }

        // Ví dụ Type = Booking, ReferenceId = BookingId. Frontend click vào sẽ redirect qua trang chi tiết Booking.
        public string? ReferenceId { get; private set; }

        // Trạng thái đã đọc hay chưa
        public bool IsRead { get; private set; }

        // Thời gian đọc 
        public DateTime? ReadAt { get; private set; }

        private Notification() { } 

        public Notification(Guid ownerId, string title, string message, NotificationType type, string referenceId)
        {
            OwnerId = ownerId;
            Title = title;
            Message = message;
            Type = type;
            ReferenceId = referenceId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }
    }
}
