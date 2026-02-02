namespace PaymentService.Domain.Enum
{
    public enum SettlementItemType: byte
    {
        Booking  = 0,       // Khoản thanh toán từ booking
        RefundDeduction = 1,       // Khoản hoàn tiền cho khách hàng
        Adjustment = 2      // Khoản điều chỉnh (cộng/trừ)
    }
}
