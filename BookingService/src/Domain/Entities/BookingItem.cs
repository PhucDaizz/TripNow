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
        public decimal Price { get; private set; } // Giá gốc tại thời điểm đặt
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

        public decimal CalculateRefund(DateTime cancelTime, DateTime checkInDate)
        {
            if (string.IsNullOrEmpty(CancellationPolicyData)) return 0;

            var rules = JsonSerializer.Deserialize<List<PolicyRuleSnapshot>>(CancellationPolicyData);
            if (rules == null || !rules.Any()) return 0;

            var checkInDateTime = checkInDate.Date.AddHours(14);
            var hoursRemaining = (checkInDateTime - cancelTime).TotalHours;

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
