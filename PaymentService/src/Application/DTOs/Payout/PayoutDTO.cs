using PaymentService.Domain.Enum;

namespace PaymentService.Application.DTOs.Payout
{
    public class PayoutDTO
    {
        public Guid Id { get; set; }
        public Guid? SettlementId { get; set; }
        public Guid OwnerWalletId { get; set; }
        public string BankInfo { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionReference { get; set; }
        public string? FailureReason { get; set; }
        public PayoutStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
