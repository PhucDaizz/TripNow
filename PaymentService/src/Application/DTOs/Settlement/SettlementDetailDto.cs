using PaymentService.Application.DTOs.SettlementItem;

namespace PaymentService.Application.DTOs.Settlement
{
    public class SettlementDetailDto : SettlementPeriodDto
    {
        public List<SettlementItemDto> Items { get; set; } = new();
    }
}
