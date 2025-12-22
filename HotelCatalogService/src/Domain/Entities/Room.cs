using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;

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
            RoomName = roomName;
            RoomTypeId = roomTypeId;
            Status = RoomStatus.Available; 
        }

        public void MarkAsDirty() => Status = RoomStatus.Dirty;
        public void MarkAsMaintain() => Status = RoomStatus.Maintain;

        public void MarkAsClean()
        {
            if (Status != RoomStatus.Dirty)
                throw new InvalidOperationException("Chỉ phòng đang dơ mới có thể dọn sạch.");
            Status = RoomStatus.Available;
        }
    }
}
