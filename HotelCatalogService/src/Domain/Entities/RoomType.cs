using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public Guid HotelId { get; private set; }
        public Guid? CancellationPolicyId { get; private set; }
        public string Name { get; private set; }
        public decimal BasePrice { get; private set; }
        public int Capacity { get; private set; }
        public decimal SizeM2 { get; private set; }

        private readonly List<RoomPrice> _prices = new();
        private readonly List<RoomTypeImage> _images = new();
        private readonly List<Room> _rooms = new();
        public IReadOnlyCollection<RoomTypeImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<RoomPrice> Prices => _prices.AsReadOnly();
        public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();
        public virtual CancellationPolicy? CancellationPolicy { get; private set; }

        private RoomType()
        {
            _images = new List<RoomTypeImage>();
            _prices = new List<RoomPrice>();
        }


        internal RoomType(Guid hotelId, string name, decimal basePrice, int capacity, decimal sizeM2): this()
        {
            HotelId = hotelId;
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
                existingPrice.UpdatePrice(price);
                return;
            }
            _prices.Add(new RoomPrice(this.Id, date, price));
        }

        public void RemovePriceForDate(DateTime date)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Date.Date == date.Date);
            if (existingPrice != null)
            {
                _prices.Remove(existingPrice);
            }
        }

        public decimal GetPriceForDate(DateTime date)
        {
            var specialPrice = _prices.FirstOrDefault(p => p.Date == date.Date);
            return specialPrice?.Price ?? BasePrice;
        }

        public void AddImage(string imageUrl, string publicId)
        {
            bool isMain = _images.Count == 0;
            _images.Add(new RoomTypeImage(this.Id, imageUrl, publicId, isMain));
        }

        public void RemoveImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img == null) return;

            _images.Remove(img);

            if (img.IsMainImage && _images.Count > 0)
            {
                _images.First().SetMain(true);
            }
        }

        public void SetMainImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img == null) return;

            foreach (var item in _images)
            {
                item.SetMain(false);
            }
            img.SetMain(true);
        }

        public void UpdateDetails(string name, decimal basePrice, int capacity, decimal sizeM2)
        {
            Name = name;
            BasePrice = basePrice;
            Capacity = capacity;
            SizeM2 = sizeM2;
        }

        public void SetBulkPrices(DateTime fromDate, DateTime toDate, decimal price, List<DayOfWeek>? specificDays = null)
        {
            if (fromDate > toDate)
                throw new ArgumentException("The start date cannot be later than the end date.");

            if (price < 0)
                throw new ArgumentException("The price must not be negative.");

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                if (specificDays != null && specificDays.Any() && !specificDays.Contains(date.DayOfWeek))
                {
                    continue;
                }

                SetSpecialPrice(date, price);
            }
        }

        public void SetPolicy(Guid policyId)
        {
            CancellationPolicyId = policyId;
        }

        public void RemovePolicy()
        {
            CancellationPolicyId = null;
        }
    }
}
