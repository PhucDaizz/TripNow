namespace BookingService.Application.DTOs.BookingPriceSnapshot
{
    public class BookingPriceSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public decimal GrossAmount { get; set; }        // Tổng tiền gốc
        public decimal PromotionAmount { get; set; }    // Tiền giảm giá
        public decimal VATAmount { get; set; }          // Thuế
        public decimal ServiceFeeAmount { get; set; }   // Phí dịch vụ
        public decimal NetPayableByCustomer { get; set; } // Tổng phải trả (Gross + Tax + Service - Promo)

        public DateTime CreatedAt { get; set; } // Ngày xuất hóa đơn
    }
}
