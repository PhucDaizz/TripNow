using BookingService.Domain.Common;
using BookingService.Domain.Exceptions;

namespace BookingService.Domain.Entities
{
    public class BookingItem : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public Guid RoomTypeId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; } // Giá gốc tại thời điểm đặt

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

        // Gán phòng vật lý (Lễ tân làm lúc Check-in)
        public void AssignRoom(Guid physicalRoomId)
        {
            if (_assignments.Count >= Quantity)
                throw new DomainException("Đã gán đủ số lượng phòng.");

            _assignments.Add(new RoomAssignment(this.Id, physicalRoomId));
        }
    }
}
