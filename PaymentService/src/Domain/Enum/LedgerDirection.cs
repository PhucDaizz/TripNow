namespace PaymentService.Domain.Enum
{
    public enum LedgerDirection : byte
    {
        Credit = 1, // Cộng tiền vào ví
        Debit = 2   // Trừ tiền khỏi ví
    }

}
