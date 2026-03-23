namespace HotelCatalogService.Application.DTOs.Hotel.Event
{
    public class HotelRoomTypeSummary
    {
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public decimal SizeM2 { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? CancellationPolicyDescription { get; set; }
    }
}
