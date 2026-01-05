using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Events.Room;

namespace HotelCatalogService.Domain.Entities
{
    public class Room: BaseEntity
    {
        public Guid FloorId { get; private set; }
        public Guid RoomTypeId { get; private set; }
        public string RoomName { get; private set; } 
        public RoomStatus Status { get; private set; }

        private Room() { }

        internal Room(Guid floorId, string roomName, Guid roomTypeId)
        {
            FloorId = floorId;
            RoomName = roomName;
            RoomTypeId = roomTypeId;
            Status = RoomStatus.Available; 
        }

        public void UpdateDetails(string roomName, Guid roomTypeId)
        {
            if(this.RoomTypeId != roomTypeId)
            {
                var oldRoomTypeId = this.RoomTypeId;
                var newRoomTypeId = roomTypeId;
                AddDomainEvent(new RoomMovedToAnotherRoomTypeEvent(oldRoomTypeId, newRoomTypeId));
            }

            RoomName = roomName;
            RoomTypeId = roomTypeId;
        }

        public void MarkAsDirty() => Status = RoomStatus.Dirty;
        public void MarkAsMaintain() => Status = RoomStatus.Maintain;

        public void MarkAsClean()
        {
            if (Status != RoomStatus.Dirty)
                throw new InvalidOperationException("Chỉ phòng đang dơ mới có thể dọn sạch.");
            Status = RoomStatus.Available;
        }

        public void StartCleaning()
        {
            if (Status != RoomStatus.Dirty)
                throw new InvalidOperationException("Chỉ phòng đang Dơ (Dirty) mới có thể bắt đầu dọn.");

            Status = RoomStatus.Cleaning;
        }

        // 2. Hoàn tất dọn (Chỉ được phép khi đang Cleaning)
        // Sửa lại hàm MarkAsClean cũ hoặc tạo hàm mới FinishCleaning
        public void FinishCleaning()
        {
            if (Status != RoomStatus.Cleaning)
                throw new InvalidOperationException("Phải chuyển sang trạng thái Đang dọn (Cleaning) trước khi hoàn tất.");

            Status = RoomStatus.Available;
        }
    }
}
