namespace HotelCatalogService.Application.DTOs.Room
{
    public class MaintainRoomRequest
    {
        public DateOnly FromDate { get; init; }
        public DateOnly ToDate { get; init; }
    }
}
