namespace PaymentService.Application.DTOs.Wallet
{
    public class WalletLedgerDto
    {
        public Guid Id { get; set; }
        public string Direction { get; set; }      // Credit (+) / Debit (-)
        public decimal Amount { get; set; }        // Số tiền biến động
        public decimal BalanceAfter { get; set; }  // Số dư sau giao dịch
        public string Type { get; set; }           // Booking, Payout, Refund...
        public string Description { get; set; }
        public Guid ReferenceId { get; set; }      // BookingId hoặc PayoutId để click vào xem chi tiết
        public DateTime CreatedAt { get; set; }
    }
}
