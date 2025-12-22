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


        internal Floor AddFloor(Guid blockId, int floorNumber)
        {
            if (_floors.Any(f => f.FloorNumber == floorNumber))
                throw new InvalidOperationException($"Tầng {floorNumber} đã tồn tại trong khu {Name}");

            var floor = new Floor(blockId, floorNumber);

            _floors.Add(floor);

            return floor;
        }
    }
}
