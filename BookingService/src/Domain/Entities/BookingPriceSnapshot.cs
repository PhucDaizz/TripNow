using BookingService.Domain.Common;

namespace BookingService.Domain.Entities
{
    public class BookingPriceSnapshot : BaseEntity, AggregateRoot
    {
        public Guid BookingId { get; private set; }
        public Guid HotelId { get; private set; } 

        public decimal GrossAmount { get; private set; }
        public decimal PromotionAmount { get; private set; }
        public decimal VATAmount { get; private set; }
        public decimal ServiceFeeAmount { get; private set; }
        public decimal NetPayableByCustomer { get; private set; }

        private BookingPriceSnapshot() {}

        public BookingPriceSnapshot(Guid bookingId, Guid hotelId, decimal gross, decimal promo, decimal vat, decimal serviceFee)
        {
            BookingId = bookingId;
            HotelId = hotelId;
            GrossAmount = gross;
            PromotionAmount = promo;
            VATAmount = vat;
            ServiceFeeAmount = serviceFee;
            NetPayableByCustomer = (gross + vat + serviceFee) - promo;
        }
    }
}
