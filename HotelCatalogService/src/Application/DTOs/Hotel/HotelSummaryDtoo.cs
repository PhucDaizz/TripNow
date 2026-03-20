namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class HotelSummaryDtoo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Rating { get; set; }
        public decimal StartingPrice { get; set; }
        public string AddressCity { get; set; }
        public string? Thumbnail { get; set; }
    }
}
