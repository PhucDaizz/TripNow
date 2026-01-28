using BookingService.Domain.Common;
using BookingService.Domain.Events.RoomAssignment;
using BookingService.Domain.Exceptions;

namespace BookingService.Domain.Entities
{
    public class RoomAssignment : BaseEntity
    {
        public Guid BookingItemId { get; private set; }
        public Guid RoomId { get; private set; }
        public string RoomName { get; private set; }
        public bool IsCheckedIn { get; private set; }
        public DateTime? CheckInTime { get; private set; }
        public DateTime? CheckOutTime { get; private set; }

        private RoomAssignment() {}

        internal RoomAssignment(Guid bookingItemId, Guid roomId, string roomName)
        {
            BookingItemId = bookingItemId;
            RoomId = roomId;
            RoomName = roomName;
            IsCheckedIn = false;
        }

        public void CheckIn()
        {
            if (IsCheckedIn) throw new DomainException("Phòng này đã Check-in rồi.");

            IsCheckedIn = true;
            CheckInTime = DateTime.UtcNow;
        }

        public void CheckOut()
        {
            if (!IsCheckedIn) throw new DomainException("Phòng này chưa Check-in nên không thể Check-out.");

            IsCheckedIn = false;
            CheckOutTime = DateTime.UtcNow;

            AddDomainEvent(new RoomCheckedOutDomainEvent
            {
                RoomId = this.RoomId
            });
        }
    }
}
