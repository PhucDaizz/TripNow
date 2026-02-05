using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Infrastructure.Settings
{
    public class ServiceFeeSettings: IServiceFeeSettings
    {
        public const string SectionName = "ServiceFee";
        public decimal Percentage { get; set; } = default!;
    }
}
