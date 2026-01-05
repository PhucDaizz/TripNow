using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Events.Room;

namespace HotelCatalogService.Domain.Entities
{
    public class Floor : BaseEntity
    {
        public Guid BlockId { get; private set; }
        public int FloorNumber { get; private set; }
        private readonly List<Room> _rooms = new();
        public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

        private Floor()
        {
            _rooms = new List<Room>();
        }


        internal Floor(Guid blockId, int floorNumber) : this()
        {
            BlockId = blockId;
            FloorNumber = floorNumber;
        }

        public void AddRoom(string roomName, Guid roomTypeId)
        {
            if (_rooms.Any(r => r.RoomName == roomName))
                throw new InvalidOperationException($"Phòng {roomName} đã tồn tại ở tầng {FloorNumber}");

            _rooms.Add(new Room(this.Id, roomName, roomTypeId));

            this.AddDomainEvent(new RoomCreatedEvent(roomTypeId));
        }

        public void UpdateDetails(int floorNumber)
        {
            FloorNumber = floorNumber;
        }

        public void RemoveRoom(Guid roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                throw new InvalidOperationException($"Phòng với Id {roomId} không tồn tại ở tầng {FloorNumber}");

            if(room.Status == RoomStatus.Occupied)
            {
                throw new InvalidOperationException($"Không thể xóa phòng {room.RoomName} vì đang có khách thuê.");
            }
            AddDomainEvent(new RoomRemovedEvent(room.RoomTypeId));
            _rooms.Remove(room);
        }
    }
}
