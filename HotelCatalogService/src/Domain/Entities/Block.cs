using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class Block : BaseEntity
    {
        public Guid HotelId { get; private set; } 
        public string Name { get; private set; } // Khu A, Khu B 
        private readonly List<Floor> _floors = new();
        public IReadOnlyCollection<Floor> Floors => _floors.AsReadOnly();
        
        private Block()
        {
            _floors = new List<Floor>();
        }

        internal Block(Guid hotelId, string name): this()
        {
            HotelId = hotelId;
            Name = name;
        }


        public Floor AddFloor(int floorNumber)
        {
            if (_floors.Any(f => f.FloorNumber == floorNumber))
                throw new InvalidOperationException($"Tầng {floorNumber} đã tồn tại trong khu {Name}");

            var newFloor = new Floor(this.Id, floorNumber);
            _floors.Add(newFloor);

            return newFloor;
        }

        public void UpdateFloor(Guid floorId, int newFloorNumber)
        {
            var floor = _floors.FirstOrDefault(f => f.Id == floorId);
            if (floor == null) 
                throw new InvalidOperationException($"Tầng với Id {floorId} không tồn tại trong khu {Name}");

            if (_floors.Any(f => f.Id != floorId && f.FloorNumber == newFloorNumber))
                throw new InvalidOperationException($"Tầng {newFloorNumber} đã tồn tại trong khu {Name}");

            floor.UpdateDetails(newFloorNumber);
        }

        public void RemoveFloor(Guid floorId)
        {
            var floor = _floors.FirstOrDefault(f => f.Id == floorId);
            if (floor == null) return;

            if (floor.Rooms.Any())
                throw new InvalidOperationException("Không thể xóa tầng đang chứa phòng. Hãy xóa phòng trước.");

            _floors.Remove(floor);
        }

        public void UpdateDetails(string name)
        {
            // Validation nếu cần
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên khu không được để trống");
            Name = name;
        }
    }
}
