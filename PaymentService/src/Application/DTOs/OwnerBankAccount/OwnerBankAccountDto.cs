namespace PaymentService.Application.DTOs.OwnerBankAccount
{
    public class OwnerBankAccountDto
    {
        public Guid OwnerId { get; set; }
        public string BankName { get; set; }      // VCB, TCB...
        public string BankAccountNumber { get; set; }
        public string BankAccountHolder { get; set; }
        public bool IsDefault { get; set; }
    }
}
