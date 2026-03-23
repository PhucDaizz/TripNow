namespace HotelCatalogService.Application.DTOs.RoomType
{
    public class CreateRoomTypeRequest
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public decimal SizeM2 { get; set; }
        public string Description { get; set; }
    }
}
