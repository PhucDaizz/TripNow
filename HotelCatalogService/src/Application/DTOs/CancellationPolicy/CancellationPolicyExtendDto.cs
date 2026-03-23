namespace HotelCatalogService.Application.DTOs.CancellationPolicy
{
    public class CancellationPolicyExtendDto: CancellationPolicyDto
    {
        public bool IsInUse { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
