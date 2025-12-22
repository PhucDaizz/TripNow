using HotelCatalogService.Domain.Common;

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

        internal void AddRoom(Guid floorId, string roomName, Guid roomTypeId)
        {
            if (_rooms.Any(r => r.RoomName == roomName))
                throw new InvalidOperationException($"Phòng {roomName} đã tồn tại ở tầng {FloorNumber}");

            _rooms.Add(new Room(floorId, roomName, roomTypeId));
        }
    }
}
