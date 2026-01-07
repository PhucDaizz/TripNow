using BookingService.Domain.Common;

namespace BookingService.Domain.Entities
{
    public class RoomAssignment : BaseEntity
    {
        public Guid BookingItemId { get; private set; }
        public Guid RoomId { get; private set; }
        public bool IsCheckedIn { get; private set; }

        private RoomAssignment() {}

        internal RoomAssignment(Guid bookingItemId, Guid roomId)
        {
            BookingItemId = bookingItemId;
            RoomId = roomId;
            IsCheckedIn = false;
        }

        public void CheckIn() => IsCheckedIn = true;
    }
}
