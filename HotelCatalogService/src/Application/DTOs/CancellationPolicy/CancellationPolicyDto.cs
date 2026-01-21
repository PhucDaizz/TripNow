using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Application.DTOs.CancellationPolicy
{
    public class CancellationPolicyDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<CancellationPolicy.CancellationRuleDto> Rules { get; set; } = new();
    }
}
