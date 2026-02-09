namespace PaymentService.Application.DTOs.Wallet
{
    public class OwnerWalletSummaryDto
    {
        public Guid Id { get; set; }
        public decimal AvailableBalance { get; set; } 
        public decimal PendingBalance { get; set; }   
        public decimal TotalBalance => AvailableBalance + PendingBalance; 
    }
}
