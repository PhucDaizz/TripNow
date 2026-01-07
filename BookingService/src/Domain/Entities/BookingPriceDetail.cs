using BookingService.Domain.Common;
using BookingService.Domain.Enum;

namespace BookingService.Domain.Entities
{
    public class BookingPriceDetail : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public PriceType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }

        private BookingPriceDetail() {}

        public BookingPriceDetail(Guid bookingId, PriceType type, decimal amount, string desc)
        {
            BookingId = bookingId;
            Type = type;
            Amount = amount;
            Description = desc;
        }
    }
}
