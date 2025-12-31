using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class RoomPrice : BaseEntity
    {
        public Guid RoomTypeId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Price { get; private set; }

        private RoomPrice() { }

        internal RoomPrice(Guid roomTypeId, DateTime date, decimal price) 
        {
            RoomTypeId = roomTypeId;
            Date = date.Date; 
            Price = price;
        }

        internal void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0) throw new ArgumentException("Price cannot be negative");
            Price = newPrice;
        }
    }
}
