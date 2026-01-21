namespace HotelCatalogService.Application.DTOs.CancellationPolicy
{
    public class CancellationRuleDto
    {
        public Guid Id { get; set; }
        public int HoursBeforeCheckIn { get; set; }
        public decimal RefundPercentage { get; set; }
    }
}
