using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; private set; }
        public decimal BasePrice { get; private set; }
        public int Capacity { get; private set; }
        public decimal SizeM2 { get; private set; }

        private readonly List<RoomPrice> _prices = new();
        public IReadOnlyCollection<RoomPrice> Prices => _prices.AsReadOnly();

        private RoomType()
        {
            _prices = new List<RoomPrice>();
        }


        internal RoomType(string name, decimal basePrice, int capacity, decimal sizeM2): this()
        {
            Name = name;
            BasePrice = basePrice;
            Capacity = capacity;
            SizeM2 = sizeM2;
        }

        public void SetSpecialPrice(DateTime date, decimal price)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Date == date.Date);
            if (existingPrice != null)
            {
                // Nếu đã có giá ngày đó thì update (Cần thêm hàm Update trong RoomPrice hoặc thay thế object mới)
                _prices.Remove(existingPrice);
            }
            _prices.Add(new RoomPrice(date, price));
        }

        public decimal GetPriceForDate(DateTime date)
        {
            var specialPrice = _prices.FirstOrDefault(p => p.Date == date.Date);
            return specialPrice?.Price ?? BasePrice;
        }
    }
}
