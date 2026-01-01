using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Domain.Entities
{
    public class Promotion : BaseEntity
    {
        public Guid HotelId { get; private set; } 
        public string Code { get; private set; } 
        public DiscountType DiscountType { get; private set; } // 1: Percent, 2: Amount
        public decimal DiscountValue { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int InitialQuantity { get; private set; }   
        public int RemainingQuantity { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsUsed => InitialQuantity > RemainingQuantity;

        public bool IsUsable
        {
            get
            {
                var now = DateTime.UtcNow;
                return IsActive
                       && RemainingQuantity > 0
                       && now >= StartDate
                       && now <= EndDate;
            }
        }

        private Promotion() { }

        internal Promotion(Guid hotelId, string code, DiscountType discountType, decimal discountValue, DateTime start, DateTime end, int qty) 
        {
            if (end < start) throw new ArgumentException("End date must be after start date.");

            if (discountType == DiscountType.Percent && discountValue > 100) throw new ArgumentException("Percentage discount cannot exceed 100%.");

            HotelId = hotelId;
            Code = code.ToUpper();
            DiscountType = discountType;
            DiscountValue = discountValue;
            StartDate = start;
            EndDate = end;

            InitialQuantity = qty;
            RemainingQuantity = qty;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return; 
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return; 

            var now = DateTime.UtcNow;
            if (now > EndDate) throw new InvalidOperationException("Cannot activate an expired promotion.");
            if (RemainingQuantity <= 0) throw new InvalidOperationException("Cannot activate a promotion with no remaining quantity.");

            IsActive = true;
        }


        internal void UpdateDetails(string code, DiscountType type, decimal value, DateTime start, DateTime end, int newTotalQty)
        {
            if (end < start) throw new ArgumentException("End date must be after start date.");

            if (IsUsed)
            {
                if (type != DiscountType || value != DiscountValue)
                {
                    throw new InvalidOperationException(
                        "Cannot change discount type or value after the promotion has been used." +
                        "Please create a new promotion or extend its duration/quantity only.");
                }
            }
            else
            {
                ValidateDiscount(type, value);
                DiscountType = type;
                DiscountValue = value;
            }

            int quantityDifference = newTotalQty - InitialQuantity;

            if (RemainingQuantity + quantityDifference < 0)
            {
                throw new InvalidOperationException(
                    $"Cannot reduce total quantity to {newTotalQty} because {InitialQuantity - RemainingQuantity} uses have already been made.");
            }

            InitialQuantity = newTotalQty;
            RemainingQuantity += quantityDifference;

            Code = code.ToUpper();
            StartDate = start;
            EndDate = end;
        }

        public void UsePromotion()
        {
            if (!IsUsable) 
            {
                if (!IsActive) throw new InvalidOperationException("Promotion is deactivated.");
                if (RemainingQuantity <= 0) throw new InvalidOperationException("Promotion has no remaining uses.");
                throw new InvalidOperationException("Promotion has not started yet or has expired.");
            }

            RemainingQuantity--;
        }

        public void RestorePromotion()
        {
            if (RemainingQuantity < InitialQuantity)
            {
                RemainingQuantity++;
            }
        }

        private void ValidateDiscount(DiscountType type, decimal value)
        {
            if (value < 0) throw new ArgumentException("Discount value cannot be negative.");
            if (type == DiscountType.Percent && value > 100)
                throw new ArgumentException("Percentage discount cannot exceed 100%.");
        }
    }
}
