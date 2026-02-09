namespace PaymentService.Application.DTOs.Settlement
{
    public class SettlementPeriodDto
    {
        public Guid Id { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string Status { get; set; }       

        public decimal TotalGross { get; set; }      
        public decimal TotalCommission { get; set; } 
        public decimal TotalNetPayable { get; set; } 

        public DateTime CreatedAt { get; set; }
    }
}
