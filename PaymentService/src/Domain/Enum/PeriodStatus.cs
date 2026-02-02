namespace PaymentService.Domain.Enum
{
    public enum PeriodStatus: byte
    {
        Processing = 0, // đang xử lý
        Open = 1,       // đã gom tiền  
        Locked = 2,     // đã chốt sổ 
        Paid = 3        // đã chi trả
    }
}
