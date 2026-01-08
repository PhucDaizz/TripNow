using BookingService.Application.Common.Interfaces;

namespace BookingService.Infrastructure.Settings
{
    public class InventorySettings: IInventorySettings
    {
        public const string SectionName = "Inventory";
        public int LookAheadDays { get; set; } = 180;
    }
}
