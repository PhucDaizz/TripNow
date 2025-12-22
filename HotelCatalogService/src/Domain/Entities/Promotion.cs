using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class Promotion : BaseEntity
    {
        public string Code { get; private set; } 
        public byte DiscountType { get; private set; } // 1: Percent, 2: Amount
        public decimal DiscountValue { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Quantity { get; private set; }

        private Promotion() { }

        internal Promotion(string code, byte discountType, decimal discountValue, DateTime start, DateTime end, int qty) 
        {
            if (end < start) throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu");

            Code = code;
            DiscountType = discountType;
            DiscountValue = discountValue;
            StartDate = start;
            EndDate = end;
            Quantity = qty;
        }

        public bool IsValid()
        {
            var now = DateTime.UtcNow;
            return now >= StartDate && now <= EndDate && Quantity > 0;
        }

        public void UsePromotion()
        {
            if (!IsValid()) throw new InvalidOperationException("Mã giảm giá hết hạn hoặc hết lượt dùng");
            Quantity--;
        }
    }
}
