using BookingService.Domain.Common;
using BookingService.Domain.Exceptions;
using BookingService.Domain.ValueObject;
using System.Text.Json;

namespace BookingService.Domain.Entities
{
    public class BookingItem : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public Guid RoomTypeId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; } // Giá gốc của mỗi phòng đã tính thuế (đã tính tiền số ngày ở) price = số tiền 1 ngày * số ngày ở
        public string CancellationPolicyName { get; private set; }
        public string CancellationPolicyData { get; private set; }

        private readonly List<RoomAssignment> _assignments = new();
        public IReadOnlyCollection<RoomAssignment> Assignments => _assignments.AsReadOnly();

        private BookingItem() {}
        internal BookingItem(Guid bookingId, Guid roomTypeId, int quantity, decimal price)
        {
            BookingId = bookingId;
            RoomTypeId = roomTypeId;
            Quantity = quantity;
            Price = price;
        }

        public void SetPolicySnapshot(string name, List<PolicyRuleSnapshot> rules)
        {
            CancellationPolicyName = name;
            CancellationPolicyData = JsonSerializer.Serialize(rules);
        }

        public decimal CalculateRefund(DateTime cancelTimeUtc, DateTime checkInDate)
        {
            if (string.IsNullOrEmpty(CancellationPolicyData)) return 0;

            var rules = JsonSerializer.Deserialize<List<PolicyRuleSnapshot>>(CancellationPolicyData);
            if (rules == null || !rules.Any()) return 0;

            // CHUẨN HÓA MÚI GIỜ (Fix lỗi hoàn 100%)
            // 1. Lấy mốc 14:00 của ngày Check-in (Giờ địa phương, ví dụ VN)
            var checkInLocal = checkInDate;

            // 2. Chuyển 14:00 VN sang UTC (Trừ đi 7 tiếng -> Thành 07:00 sáng UTC)
            // Nếu KS ở quốc gia khác, bác có thể thay số 7 bằng Offset tương ứng của khách sạn
            var checkInUtc = checkInLocal.AddHours(-7);

            var hoursRemaining = (checkInUtc - cancelTimeUtc).TotalHours;

            if (hoursRemaining < 0) return 0;

            var matchedRule = rules
                .OrderByDescending(r => r.HoursBeforeCheckIn)
                .FirstOrDefault(r => hoursRemaining >= r.HoursBeforeCheckIn);

            if (matchedRule == null) return 0;

            decimal totalItemPrice = Price * Quantity;
            return totalItemPrice * (matchedRule.RefundPercentage / 100m);
        }

        // Gán phòng vật lý (Lễ tân làm lúc Check-in)
        public void AssignRoom(Guid roomId, string roomName)
        {
            if (_assignments.Count >= Quantity)
                throw new DomainException("Đã gán đủ số lượng phòng.");

            if (_assignments.Any(x => x.RoomId == roomId))
            {
                throw new DomainException("Phòng này đã được gán cho khách này rồi.");
            }

            var assignment = new RoomAssignment(this.Id, roomId, roomName);
            _assignments.Add(assignment);
        }
    }
}
