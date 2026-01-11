using BookingService.Domain.Common;

namespace BookingService.Domain.Entities
{
    public class InventoryConfiguration: BaseEntity
    {
        public Guid HotelId { get; private set; }
        public Guid RoomTypeId { get; private set; }
        public bool IsActive { get; private set; } 
        public int DefaultStock { get; private set; } 
        public DateTime? LastGeneratedDate { get; private set; }

        private InventoryConfiguration() { }

        public static InventoryConfiguration Create(Guid hotelId, Guid roomTypeId, int defaultStock)
        {
            return new InventoryConfiguration
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                DefaultStock = defaultStock,
                IsActive = true,
                LastGeneratedDate = null // Chưa chạy job lần nào
            };
        }

        /// <summary>
        /// Cập nhật ngày cuối cùng đã tạo Inventory.
        /// Hàm này đảm bảo chỉ tịnh tiến về phía trước, không bao giờ thụt lùi.
        /// </summary>
        /// <param name="targetDate">Ngày vừa mới tạo xong (DateOnly)</param>
        public void UpdateLastGeneratedDate(DateOnly targetDate)
        {
            // Convert DateOnly sang DateTime (Đầu ngày 00:00:00)
            var newDate = targetDate.ToDateTime(TimeOnly.MinValue);

            // Logic an toàn: Chỉ update nếu ngày mới LỚN HƠN ngày cũ
            // Tránh trường hợp chạy lại Job cũ làm sai lệch dữ liệu
            if (LastGeneratedDate == null || newDate > LastGeneratedDate)
            {
                LastGeneratedDate = newDate;
            }
        }

        /// <summary>
        /// Tạm ngưng hoạt động (Khi khách sạn đóng cửa)
        /// </summary>
        public void MarkAsInactive()
        {
            IsActive = false;
        }

        /// <summary>
        /// Kích hoạt lại (Khi khách sạn mở cửa)
        /// </summary>
        public void MarkAsActive()
        {
            IsActive = true;
        }

        /// <summary>
        /// Cập nhật số lượng mặc định (nếu chủ khách sạn đổi cấu hình)
        /// </summary>
        public void UpdateDefaultStock(int newStock)
        {
            if (newStock < 0) throw new ArgumentException("Stock cannot be negative");
            DefaultStock = newStock;
        }
    }
}
